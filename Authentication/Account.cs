using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Clifton.IO.Authentication {
    public class Account {
        #region Object definition

        public Guid id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string hash { get; set; }

        // When serializing on a retrieval, the hash field shouldn't be sent back to the caller.
        // http://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        private bool shouldSerializeHash;
        public bool ShouldSerializehash() { return shouldSerializeHash; }

        public DateTime created { get; set; }
        public AccountStatus? accountStatus { get; set; }

        public Account() {
            shouldSerializeHash = true;
        }
        public Account(DataRow dr) {
            shouldSerializeHash = false;
            id = (Guid)dr["id"];
            name = (string)dr["name"];
            email = (string)dr["email"];
            hash = (string)dr["hash"];
            created = (DateTime)dr["created"];
            if (dr["accountStatusId"] == DBNull.Value) {
                accountStatus = null;
            } else {
                accountStatus = (AccountStatus)dr["accountStatusId"];
            }
        }

        #endregion

        private static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["Default"].ConnectionString; } }

        #region CRUD

        public static void Insert(Account value, out Guid code) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountCreate",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("name", SqlDbType.NVarChar, 255).Value = value.name;
                cmd.Parameters.Add("email", SqlDbType.NVarChar, 255).Value = value.email;
                cmd.Parameters.Add("hash", SqlDbType.NVarChar, 100).Value = PasswordHash.CreateHash(value.hash);
                cmd.Parameters.Add("accountId", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("code", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                value.id = (Guid)cmd.Parameters["accountId"].Value;
                code = (Guid)cmd.Parameters["code"].Value;

                cn.Close();
            }
        }

        public static List<Account> Select() {
            string query = string.Format("SELECT * FROM viewAccounts");

            using (SqlDataAdapter da = new SqlDataAdapter(query, ConnectionString)) {
                DataTable dt = new DataTable();
                int count = da.Fill(dt);

                List<Account> accounts = new List<Account>();
                foreach (DataRow dr in dt.Rows) {
                    accounts.Add(new Account(dr));
                }

                return accounts;
            }
        }

        public static Account Select(Guid id) {
            string query = string.Format("SELECT * FROM viewAccounts WHERE id = '{0}'", id);

            using (SqlDataAdapter da = new SqlDataAdapter(query, ConnectionString)) {
                DataTable dt = new DataTable();
                int count = da.Fill(dt);

                if (count != 1) return null;

                return new Account(dt.Rows[0]);
            }
        }

        public static bool Delete(Guid id) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountRemove",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("accountId", SqlDbType.UniqueIdentifier).Value = id;

                cn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cn.Close();

                if (rowsAffected > 0) {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Actions

        public static Account Authenticate(string email, string password) {
            string query = string.Format("SELECT * FROM viewAccounts WHERE email = '{0}'", email);

            using (SqlDataAdapter da = new SqlDataAdapter(query, ConnectionString)) {
                DataTable dt = new DataTable();
                if (da.Fill(dt) == 1) {
                    Account account = new Account(dt.Rows[0]);
                    if (PasswordHash.ValidatePassword(password, account.hash)) {
                        return account;
                    }
                }
            }

            return null;
        }

        public static bool Confirm(Guid accountId, Guid code, string clientId) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountConfirm",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("accountId", SqlDbType.UniqueIdentifier).Value = accountId;
                cmd.Parameters.Add("code", SqlDbType.UniqueIdentifier).Value = code;
                cmd.Parameters.Add("clientId", SqlDbType.NVarChar, 50).Value = clientId;

                cn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cn.Close();

                if (rowsAffected > 0) {
                    return true;
                }

                return false;
            }
        }

        #endregion
    }
}