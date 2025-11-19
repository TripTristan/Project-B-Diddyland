using System;
using System.Data;
using Dapper;

public static class GroupPaymentRepository
{

    public static void InsertGroupPayment(string orderNumber, decimal amount, string method, string confirmationNumber)
    {
        const string sql = @"
            INSERT INTO GroupPayment (OrderNumber, Amount, Method, ConfirmationNumber, PaidAt)
            VALUES (@OrderNumber, @Amount, @Method, @ConfirmationNumber, GETDATE())";
            DBC.Connection.Execute(sql, new { orderNumber, amount, method, confirmationNumber });
    }


    public static bool IsOrderPaid(string orderNumber)
    {
        const string sql = "SELECT COUNT(1) FROM GroupPayment WHERE OrderNumber = @OrderNumber";
        return DBC.Connection.ExecuteScalar<int>(sql, new { OrderNumber = orderNumber }) > 0;
    }


    public static bool SimulateGateway() => new Random().Next(100) < 95;
}
