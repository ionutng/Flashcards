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
                        ManageFlashcards();
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

                string stackName = Validation.GetString("Please insert the name of the stack or Type 0 to return to the main menu.");

                CheckDuplicate(stackName);

                string query = $"INSERT INTO Stacks (Name) VALUES ('"+ stackName +"')";

                SqlCommand command = new(query, connection);

                command.ExecuteNonQuery();

                Console.Clear();
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
            Console.Clear();
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

                Console.WriteLine();
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

            string stackId = Validation.GetString("Please type the id of the stack you would like to delete or Type 0 to return to the main menu.");

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"DELETE FROM Stacks WHERE StackId = {stackId}";

                SqlCommand command = new(query, connection);

                Console.Clear();
                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"Stack #{stackId} successfully deleted!\n");
                else
                    Console.WriteLine($"A stack with id: {stackId} doesn't exist!\n");

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

            string stackId = Validation.GetString("Please type the id of the stack you would like to update or Type 0 to return to the main menu.");

            string stackName = Validation.GetString("Please type the new name of the stack or Type 0 to return to the main menu.");

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"UPDATE Stacks SET Name = ('" + stackName + "') WHERE StackId = '" + stackId + "'";

                SqlCommand command = new(query, connection);

                Console.Clear();
                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"Stack #{stackId} successfully updated!\n");
                else
                    Console.WriteLine($"A stack with id: {stackId} doesn't exist!\n");

                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ManageFlashcards()
        {
            Console.Clear();
            Console.WriteLine("| Flashcards Menu |\n");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("\n--------------------------------------");
            Console.WriteLine("Type A to Add a Flashcard.");
            Console.WriteLine("Type V to View all Flashcard.");
            Console.WriteLine("Type D to Delete a Flashcard.");
            Console.WriteLine("Type U to Update a Flashcard.");
            Console.WriteLine("--------------------------------------");

            string flashcardOption = Console.ReadLine();

            switch (flashcardOption.Trim().ToLower())
            {
                case "a":
                    InsertFlashcard();
                    break;
                case "v":
                    GetFlashcards();
                    break;
                case "d":
                    DeleteFlashcard();
                    break;
                case "u":
                    UpdateFlashcard();
                    break;
                default:
                    Console.WriteLine("Not an available option. Please try again!");
                    break;
            }
        }

        static void InsertFlashcard()
        {
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string flashcardQuestion = Validation.GetString("Please insert the question for the flashcard or Type 0 to return to the main menu.");
                string flashcardAnswer = Validation.GetString("Please insert the correct answer for the question or Type 0 to return to the main menu.");

                Console.WriteLine();
                GetStacks();
                string stackId = Validation.GetString("Please insert the id of the stack the flashcard should be in or Type 0 to return to the main menu.");

                string query = $"INSERT INTO Flashcards (Question, Answer, StackId) VALUES ('" + flashcardQuestion + "', '" + flashcardAnswer + "', '" + stackId + "')";

                SqlCommand command = new(query, connection);

                command.ExecuteNonQuery();

                Console.Clear();
                Console.WriteLine($"Successfully added a flashcard!");

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void GetFlashcards()
        {
            Console.Clear();
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT FlashcardId, Question, Answer, Name FROM Flashcards " +
                    $"INNER JOIN Stacks ON Flashcards.StackId = Stacks.StackId";

                SqlCommand command = new(query, connection);

                var tableData = new DataTable();
                tableData.Load(command.ExecuteReader());

                connection.Close();

                if (tableData.Rows.Count > 0)
                {
                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle("Flashcards", ConsoleColor.DarkBlue, ConsoleColor.DarkGray)
                        .WithColumn("Id", "Question", "Answer", "Stack Name")
                        .WithTextAlignment(new Dictionary<int, TextAligntment>
                        {
                            {0, TextAligntment.Center },
                            {1, TextAligntment.Center },
                            {2, TextAligntment.Center },
                            {3, TextAligntment.Center },
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
                
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DeleteFlashcard()
        {
            GetFlashcards();
            SqlConnection connection;

            string flashcardId = Validation.GetString("Please type the id of the flashcaard you would like to delete or Type 0 to return to the main menu.");

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"DELETE FROM Flashcards WHERE FlashcardId = {flashcardId}";

                SqlCommand command = new(query, connection);

                Console.Clear();
                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"Flashcard #{flashcardId} successfully deleted!\n");
                else
                    Console.WriteLine($"A flashcard with id: {flashcardId} doesn't exist!\n");

                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void UpdateFlashcard()
        {
            Console.Clear();
            Console.WriteLine("What do you want to update?");
            Console.WriteLine("Type 1 for question.");
            Console.WriteLine("Type 2 for answer.");
            Console.WriteLine("Type 3 for stack name.");
            Console.WriteLine("Type 4 for all of them.");
            Console.WriteLine("Type any other key to return to the main menu.");

            string updateChoice = Console.ReadLine();

            if (updateChoice == "1")
            {
                GetFlashcards();
                SqlConnection connection;

                string flashcardId = Validation.GetString("Please type the id of the flashcard you would like to update or Type 0 to return to the main menu.");

                string flashcardQuestion = Validation.GetString("Please type the new question or Type 0 to return to the main menu.");

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string query = $"UPDATE Flashcards SET Question = ('" + flashcardQuestion + "') WHERE FlashcardId = '" + flashcardId + "'";

                    SqlCommand command = new(query, connection);

                    Console.Clear();
                    if (command.ExecuteNonQuery() > 0)
                        Console.WriteLine($"Flashcard #{flashcardId} successfully updated!\n");
                    else
                        Console.WriteLine($"A flashcard with id: {flashcardId} doesn't exist!\n");

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (updateChoice == "2")
            {
                GetFlashcards();
                SqlConnection connection;

                string flashcardId = Validation.GetString("Please type the id of the flashcard you would like to update or Type 0 to return to the main menu.");

                string flashcardAnswer = Validation.GetString("Please type the new correct answer or Type 0 to return to the main menu.");

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string query = $"UPDATE Flashcards SET Answer = ('" + flashcardAnswer + "') WHERE FlashcardId = '" + flashcardId + "'";

                    SqlCommand command = new(query, connection);

                    Console.Clear();
                    if (command.ExecuteNonQuery() > 0)
                        Console.WriteLine($"Flashcard #{flashcardId} successfully updated!\n");
                    else
                        Console.WriteLine($"A flashcard with id: {flashcardId} doesn't exist!\n");

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (updateChoice == "3")
            {
                GetFlashcards();
                SqlConnection connection;

                string flashcardId = Validation.GetString("Please type the id of the flashcard you would like to update or Type 0 to return to the main menu.");

                Console.WriteLine();
                GetStacks();
                string stackId = Validation.GetString("Please type the new stack id or Type 0 to return to the main menu.");

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string query = $"UPDATE Flashcards SET StackId = ('" + stackId + "') WHERE FlashcardId = '" + flashcardId + "'";

                    SqlCommand command = new(query, connection);

                    Console.Clear();
                    if (command.ExecuteNonQuery() > 0)
                        Console.WriteLine($"Flashcard #{flashcardId} successfully updated!\n");
                    else
                        Console.WriteLine($"A flashcard with id: {flashcardId} doesn't exist!\n");

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (updateChoice == "4")
            {
                GetFlashcards();
                SqlConnection connection;

                string flashcardId = Validation.GetString("Please type the id of the flashcard you would like to update or Type 0 to return to the main menu.");

                string flashcardQuestion = Validation.GetString("Please type the new question or Type 0 to return to the main menu.");

                string flashcardAnswer = Validation.GetString("Please type the new correct answer or Type 0 to return to the main menu.");

                GetStacks();
                string stackId = Validation.GetString("Please type the new stack id or Type 0 to return to the main menu.");

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string query = $"UPDATE Flashcards SET " +
                        "Question = ('" + flashcardQuestion + "'), " +
                        "Answer = ('" + flashcardAnswer + "'), " +
                        "StackId = ('" + stackId + "') " +
                        "WHERE FlashcardId = '" + flashcardId + "'";

                    SqlCommand command = new(query, connection);

                    Console.Clear();
                    if (command.ExecuteNonQuery() > 0)
                        Console.WriteLine($"Flashcard #{flashcardId} successfully updated!\n");
                    else
                        Console.WriteLine($"A flashcard with id: {flashcardId} doesn't exist!\n");

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.Clear();
                GetUserInput();
            }
        }

        static void CheckDuplicate(string name)
        {
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT * FROM Stacks WHERE Name = ('"+ name +"')";

                SqlCommand command = new(query, connection);

                var tableData = new DataTable();
                tableData.Load(command.ExecuteReader());

                if (tableData.Rows.Count > 0)
                {
                    Console.Clear();
                    Console.WriteLine($"A Stack with the name {name} already exists!");
                    GetUserInput();
                }    

                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
