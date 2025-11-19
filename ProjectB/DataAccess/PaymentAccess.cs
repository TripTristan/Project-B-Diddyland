using System;
using System.Data;
using Dapper;

namespace MyProject.DAL
{
    public static class PaymentRepository
    {

        public static void InsertPayment(string orderNumber, decimal amount, string method, string confirmationNumber)
        {
            const string sql = @"
                INSERT INTO Payment (OrderNumber, Amount, Method, ConfirmationNumber, PaidAt)
                VALUES (@OrderNumber, @Amount, @Method, @ConfirmationNumber, GETDATE())";
                DBC.Connection.Execute(sql, new { orderNumber, amount, method, confirmationNumber });
        }
        public static bool SimulateGateway(PaymentMethods method) => new Random().Next(100) < 95;
    }
}