using ConsoleTableExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards
{
    internal class DatabaseManager
    {
        static readonly string connectionString = @"Data Source=(LocalDb)\LocalDB;Initial Catalog=Flashcards;Integrated Security=True";

        internal static void GetUserInput()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Flashcards app!");
                Console.WriteLine("\n--------------------------------------");
                Console.WriteLine("Type 1 to Manage Stacks.");
                Console.WriteLine("Type 2 to Manage Flashcards.");
                Console.WriteLine("Type 3 to Start a Study Session.");
                Console.WriteLine("Type 4 to View the Study Session Data.");
                Console.WriteLine("Type 0 to Exit the Application.");
                Console.WriteLine("--------------------------------------");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        ManageStacks();
                        break;
                    case "2":
                        Console.WriteLine("\nManaging flashcards!");
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nWrong input! Please type a number between 0 and 2.");
                        break;
                }
            }
        }

        static void ManageStacks()
        {
            Console.Clear();
            Console.WriteLine("| Stack Menu |\n");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("\n--------------------------------------");
            Console.WriteLine("Type A to Add a Stack.");
            Console.WriteLine("Type V to View all Stacks.");
            Console.WriteLine("Type D to Delete a Stack.");
            Console.WriteLine("Type U to Update a Stack.");
            Console.WriteLine("--------------------------------------");

            string stacksOption = Console.ReadLine();

            switch (stacksOption.Trim().ToLower())
            {
                case "a":
                    InsertStack();
                    break;
                case "v":
                    GetStacks();
                    break;
                case "d":
                    DeleteStack();
                    break;
                case "u":
                    UpdateStack();
                    break;
                default:
                    Console.WriteLine("Not an available option. Please try again!");
                    break;
            }
        }

        static void InsertStack()
        {
            SqlConnection connection;
            
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                Console.WriteLine("\nPlease insert the name of the stack.");
                string stackName = Console.ReadLine();

                string query = $"INSERT INTO Stacks (Name) VALUES ('"+ stackName +"')";

                SqlCommand command = new(query, connection);

                command.ExecuteNonQuery();

                Console.WriteLine($"Successfully added a {stackName} stack!");

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void GetStacks()
        {
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT * FROM Stacks";

                SqlCommand command = new(query, connection);

                var tableData = new DataTable();
                tableData.Load(command.ExecuteReader());

                connection.Close();

                if (tableData.Rows.Count > 0)
                {
                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle("Stacks", ConsoleColor.DarkBlue, ConsoleColor.DarkGray)
                        .WithColumn("Id", "Name")
                        .WithTextAlignment(new Dictionary<int, TextAligntment>
                        {
                            {0, TextAligntment.Center },
                            {1, TextAligntment.Center },
                        })
                        .WithCharMapDefinition(new Dictionary<CharMapPositions, char> {
                            {CharMapPositions.BottomLeft, '=' },
                            {CharMapPositions.BottomCenter, '=' },
                            {CharMapPositions.BottomRight, '=' },
                            {CharMapPositions.BorderTop, '=' },
                            {CharMapPositions.BorderBottom, '=' },
                            {CharMapPositions.BorderLeft, '|' },
                            {CharMapPositions.BorderRight, '|' },
                            {CharMapPositions.DividerY, '|' },
                        })
                        .WithHeaderCharMapDefinition(new Dictionary<HeaderCharMapPositions, char> {
                            {HeaderCharMapPositions.TopLeft, '=' },
                            {HeaderCharMapPositions.TopCenter, '=' },
                            {HeaderCharMapPositions.TopRight, '=' },
                            {HeaderCharMapPositions.BottomLeft, '|' },
                            {HeaderCharMapPositions.BottomCenter, '-' },
                            {HeaderCharMapPositions.BottomRight, '|' },
                            {HeaderCharMapPositions.Divider, '|' },
                            {HeaderCharMapPositions.BorderTop, '=' },
                            {HeaderCharMapPositions.BorderBottom, '-' },
                            {HeaderCharMapPositions.BorderLeft, '|' },
                            {HeaderCharMapPositions.BorderRight, '|' },
                        })
                        .ExportAndWriteLine();
                }
                else
                    Console.WriteLine("\nThere are no records yet!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DeleteStack()
        {
            GetStacks();
            SqlConnection connection;

            Console.WriteLine("\nPlease type the id of the stack you would like to delete.");
            string stackId = Console.ReadLine();

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"DELETE FROM Stacks WHERE StackId = {stackId}";

                SqlCommand command = new(query, connection);

                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"Stack #{stackId} successfully deleted!");
                else
                    Console.WriteLine($"A stack with id: {stackId} doesn't exist!");

                connection.Close();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void UpdateStack()
        {
            GetStacks();
            SqlConnection connection;

            Console.WriteLine("\nPlease type the id of the stack you would like to update.");
            string stackId = Console.ReadLine();

            Console.WriteLine("\nPlease type the new name of the stack.");
            string stackName = Console.ReadLine();

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"UPDATE Stacks SET Name = ('" + stackName + "') WHERE StackId = '"+ stackId + "'";

                SqlCommand command = new(query, connection);

                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"Stack #{stackId} successfully updated!");
                else
                    Console.WriteLine($"A stack with id: {stackId} doesn't exist!");

                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
