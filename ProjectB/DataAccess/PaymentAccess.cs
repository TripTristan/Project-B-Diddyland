using System;
using System.Data;
using Dapper;

namespace MyProject.DAL
{
    public static class PaymentRepository
    {

        public static void InsertPayment(string orderNumber, decimal amount, string method, string confirmationNumber)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = @"
                    INSERT INTO Payment (OrderNumber, Amount, Method, ConfirmationNumber, PaidAt)
                    VALUES (@OrderNumber, @Amount, @Method, @ConfirmationNumber, datetime('now'))";
                DBC.Connection.Execute(sql, new { orderNumber, amount, method, confirmationNumber });
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }
        public static bool SimulateGateway(PaymentMethods method) => new Random().Next(100) < 95;
    }
}