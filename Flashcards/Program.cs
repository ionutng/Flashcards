using System.Data.SqlClient;

SqlConnection sqlConnection;

string connectionString = @"Data Source=(LocalDb)\LocalDB;Initial Catalog=Flashcards;Integrated Security=True";

try
{
    sqlConnection = new SqlConnection(connectionString);

    sqlConnection.Open();

    Console.WriteLine("Connected!");
} 
catch (Exception e)
{
    Console.WriteLine(e.Message);
}