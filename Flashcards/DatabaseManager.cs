using ConsoleTableExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                    case "3":
                        CreateStudySession();
                        break;
                    case "4":
                        ViewStudySessions();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nWrong input! Please type a number between 0 and 4.");
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
                    GetStack();
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

                var tableData = new List<List<object>>();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new List<object> { reader.GetString(1) });
                    }
                }

                connection.Close();

                if (tableData.Count > 0)
                {
                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle("Stacks", ConsoleColor.DarkBlue, ConsoleColor.DarkGray)
                        .WithColumn("Name")
                        .WithMinLength(new Dictionary<int, int> {
                            { 0, 10 },
                        })
                        .WithTextAlignment(new Dictionary<int, TextAligntment>
                        {
                            {0, TextAligntment.Center },
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

        static void GetStack()
        {
            string stackName = Validation.GetString("Type the stack name that you want to see or Type 0 to return to the main menu.");

            Console.Clear();
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT FlashcardId, Question, Answer FROM Stacks " +
                    $"INNER JOIN Flashcards ON Stacks.StackId = Flashcards.StackId " +
                    $"WHERE Name = '"+ stackName +"'";

                SqlCommand command = new(query, connection);

                var tableData = new List<List<object>>();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new List<object> { reader.GetInt32(0), reader.GetString(1), reader.GetString(2) });
                    }
                }

                if (tableData.Count > 0)
                {
                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle($"{stackName}", ConsoleColor.DarkBlue, ConsoleColor.DarkGray)
                        .WithColumn("FlashcardId", "Question", "Answer")
                        .WithTextAlignment(new Dictionary<int, TextAligntment>
                        {
                            {0, TextAligntment.Center },
                            {1, TextAligntment.Center },
                            {2, TextAligntment.Center },
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

                connection.Close();
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

            string stackName = Validation.GetString("Please type the name of the stack you would like to delete or Type 0 to return to the main menu.");

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"DELETE FROM Stacks WHERE Name = '" + stackName + "'";

                SqlCommand command = new(query, connection);

                Console.Clear();
                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"{stackName} stack successfully deleted!\n");
                else
                    Console.WriteLine($"{stackName} stack doesn't exist!\n");

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

            string oldStackName = Validation.GetString("Please type the name of the stack you would like to update or Type 0 to return to the main menu.");

            string stackName = Validation.GetString("Please type the new name of the stack or Type 0 to return to the main menu.");

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"UPDATE Stacks SET Name = ('" + stackName + "') WHERE Name = '" + oldStackName + "'";

                SqlCommand command = new(query, connection);

                Console.Clear();
                if (command.ExecuteNonQuery() > 0)
                    Console.WriteLine($"{stackName} stack successfully updated!\n");
                else
                    Console.WriteLine($"{oldStackName} stack doesn't exist!\n");

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
                string stackName = Validation.GetString("Please insert the name of the stack the flashcard should be in or Type 0 to return to the main menu.");

                int stackId = GetStackId(stackName);

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

            string flashcardId = Validation.GetString("Please type the id of the flashcard you would like to delete or Type 0 to return to the main menu.");

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
                string stackName = Validation.GetString("Please type the new stack name or Type 0 to return to the main menu.");

                int stackId = GetStackId(stackName);

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
                string stackName = Validation.GetString("Please type the new stack name or Type 0 to return to the main menu.");

                int stackId = GetStackId(stackName);

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

        static int GetStackId(string stackName)
        {
            SqlConnection connection;
            int stackId = 0;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT StackId FROM Stacks WHERE Name = ('" + stackName + "')";

                SqlCommand command = new(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        stackId = reader.GetInt32(0);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return stackId;
        }

        static bool CheckStackName(string name)
        {
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT Name FROM Stacks";

                SqlCommand command = new(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                var tableData = new List<string>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(reader.GetString(0));
                    }
                }

                connection.Close();

                foreach (var data in tableData)
                    if (data.ToLower() == name.ToLower())
                        return true;

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        static void CreateStudySession()
        {
            GetStacks();
            string stackName = Validation.GetString("Type the name of the stack you want to study or Type 0 to return to the main menu.");
            
            if (CheckStackName(stackName))
            {
                SqlConnection connection;

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();
                    string query;

                    query = $"SELECT Name, Question, Answer FROM Stacks " +
                        $"INNER JOIN Flashcards ON Stacks.StackId = Flashcards.StackId " +
                        $"WHERE Name = '" + stackName + "'";

                    SqlCommand command = new(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    var tableData = new List<Flashcards>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tableData.Add(new Flashcards
                            {
                                Name = reader.GetString(0),
                                Question = reader.GetString(1),
                                Answer = reader.GetString(2)
                            });
                        }
                    }

                    string continueSession = "";
                    int questions = 0;
                    int points = 0;
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    do
                    {
                        foreach (var data in tableData)
                        {
                            Console.WriteLine($"Current stack: {data.Name}");
                            Console.WriteLine("--------------------------");
                            Console.WriteLine($"Question: {data.Question}");
                            string answer = Validation.GetString("Your answer:");
                            if (answer.Trim().ToLower() == data.Answer.ToLower())
                            {
                                Console.WriteLine("Correct! You get 1 point!");
                                points++;
                            }
                            else
                                Console.WriteLine($"Wrong! The correct answer is: {data.Answer}");
                            questions++;
                        
                            continueSession = Validation.GetString("Do you wish to keep going? y for yes");
                            if (continueSession != "y")
                                break;
                        }

                    } while (continueSession.Trim().ToLower() == "y");

                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;

                    Console.Clear();
                    Console.WriteLine($"You have answered correctly {points} out of {questions} questions.");
                    Console.WriteLine($"It took you {elapsedTime.Hours} hours, {elapsedTime.Minutes} minutes and {elapsedTime.Seconds} seconds.");

                    connection.Close();

                    InsertStudySession(points, elapsedTime, questions, stackName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (stackName == "0")
            {
                Console.Clear();
                GetUserInput();
            } 
            else
            {
                Console.Clear();
                Console.WriteLine($"A stack with name: {stackName} doesn't exist!");
                GetUserInput();
            }
        }

        static void InsertStudySession(int points, TimeSpan elapsedTime, int questions, string stackName)
        {
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO StudySessions VALUES ('" + DateTime.Now + "', '" + points + "', '" + elapsedTime + "', '" + questions + "', '" + GetStackId(stackName) + "')";

                SqlCommand command = new(query, connection);

                command.ExecuteNonQuery();

                Console.WriteLine($"Successfully added a study session!");

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ViewStudySessions()
        {
            Console.Clear();
            SqlConnection connection;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string query = $"SELECT Date, Score, TotalQuestions, Time, Name FROM StudySessions " +
                    $"INNER JOIN Stacks ON StudySessions.StackId = Stacks.StackId";

                SqlCommand command = new(query, connection);

                var tableData = new List<List<object>>();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new List<object> { 
                            reader.GetDateTime(0).ToString("dd-MM-yyyy"), 
                            reader.GetInt32(1), 
                            reader.GetInt32(2), 
                            reader.GetTimeSpan(3).Hours + ":" + reader.GetTimeSpan(3).Minutes + ":" + reader.GetTimeSpan(3).Seconds, 
                            reader.GetString(4) 
                        });
                    }
                }

                connection.Close();

                if (tableData.Count > 0)
                {
                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle("Study Sessions", ConsoleColor.DarkBlue, ConsoleColor.DarkGray)
                        .WithColumn("Date", "Score", "Questions", "Time (H:m:s)", "Stack Name")
                        .WithTextAlignment(new Dictionary<int, TextAligntment>
                        {
                            {0, TextAligntment.Center },
                            {1, TextAligntment.Center },
                            {2, TextAligntment.Center },
                            {3, TextAligntment.Center },
                            {4, TextAligntment.Center },
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
    }
    class Flashcards
    {
        internal string Name { get; set; }

        internal string Question { get; set; }

        internal string Answer { get; set; }
    }
}
