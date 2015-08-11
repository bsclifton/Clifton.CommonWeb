using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Clifton.CommonWeb.Authentication {
    public class AccountSession {
        private static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["Default"].ConnectionString; } }

        #region Actions

        public static void Begin(Guid accountId, string clientId, bool rememberMe, out Guid sessionId, out DateTime? expires) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountSessionBegin",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("accountId", SqlDbType.UniqueIdentifier).Value = accountId;
                cmd.Parameters.Add("clientId", SqlDbType.NVarChar, 50).Value = clientId;
                cmd.Parameters.Add("sessionId", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("expires", SqlDbType.DateTime).Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                sessionId = (Guid)cmd.Parameters["sessionId"].Value;
                if (rememberMe) {
                    expires = (DateTime)cmd.Parameters["expires"].Value;
                } else {
                    expires = null;
                }

                cn.Close();
            }
        }

        public static void End(Guid sessionId) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountSessionEnd",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("sessionId", SqlDbType.UniqueIdentifier).Value = sessionId;

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public static bool Validate(Guid accountId, string clientId, Guid sessionId) {
            using (SqlConnection cn = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    Connection = cn,
                    CommandText = "spAccountSessionValidate",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("accountId", SqlDbType.UniqueIdentifier).Value = accountId;
                cmd.Parameters.Add("clientId", SqlDbType.NVarChar, 50).Value = clientId;
                cmd.Parameters.Add("sessionId", SqlDbType.UniqueIdentifier).Value = sessionId;
                cmd.Parameters.Add("expires", SqlDbType.DateTime).Direction = ParameterDirection.Output;

                cn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                DateTime? expires = null;
                
                if (cmd.Parameters["expires"].Value != DBNull.Value) {
                    expires = (DateTime)cmd.Parameters["expires"].Value;
                }

                cn.Close();

                return expires.HasValue && expires > DateTime.UtcNow;
            }
        }

        #endregion
    }
}