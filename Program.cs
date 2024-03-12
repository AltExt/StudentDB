using System.Data.Entity;
using System.Runtime.InteropServices;

namespace StudentDB
{
	internal class Program
	{
		static public List<string> options = ["View Students", "Add Student", "Remove student", "Exit"];
		
		static void Main(string[] args)
		{
			bool running = true;
			while (running)
			{
				DisplayOptions();
				running = GetAndHandleUserInput();
			}
		}

		public static void DisplayOptions()
		{
			for (int i = 0; i < options.Count; i++) Console.WriteLine(i.ToString() + ")  " + options[i]);
		}

		public static bool GetAndHandleUserInput()
		{
			int userSelection = MyUtils.ConsoleFunctions.GetIntFromUserWithBounds(0, options.Count-1, "Please Choose from the above options", "Please enter a number between 0 and " + (options.Count-1).ToString());
			switch (userSelection)
			{
				case 0:
					DisplayStudents();
					Console.WriteLine("Press any key to continue...");
					Console.ReadKey();
					break;
				case 1:
					AddStudent();
					break;
				case 2:
					RemoveStudent();
					break;
				case 3:
					return false;
			}
			Console.Clear();
			return true;
		}

		private static void DisplayStudents()
		{
			List<Student> students = new()
			{
				new Student()
				{
					FirstName = "First Name",
					LastName = "Last Name"
				}
			};

			using (var db = new StudentDbContext())
			{
				var query = from s in db.Students
							orderby s.Id
							select s;

				students.AddRange(query.ToList());
			}
			
			if (students.Count > 0)
			{
				Console.Clear();
				int maxID = students.Max(s => s.Id.ToString().Length) + 2;
				int maxFirstNameLen = students.Max(s => s.FirstName.Length) + 2;
				int maxLastNameLen = students.Max(s => s.LastName.Length) + 2;
				int[] heights = new int[students.Count];
				for (int i = 0; i < students.Count; i++) heights[i] = 1;
				int[] widths = [maxID, maxFirstNameLen, maxLastNameLen];
				MyUtils.Console_FlexGrid g = new MyUtils.Console_FlexGrid(0, 0, widths, heights);
				g.DrawGrid();

				for (int i = 0; i < students.Count; i++)
				{
					int x = g.GetCellAt(0, i).startX + 1;
					int y = g.GetCellAt(0, i).startY;
					if (i == 0) MyUtils.ConsoleFunctions.WriteStringAtPosition("ID", x, y);
					else MyUtils.ConsoleFunctions.WriteStringAtPosition(students[i].Id.ToString(), x, y);
					x = g.GetCellAt(1, i).startX + 1;
					y = g.GetCellAt(1, i).startY;
					MyUtils.ConsoleFunctions.WriteStringAtPosition(students[i].FirstName, x, y);
					x = g.GetCellAt(2, i).startX + 1;
					y = g.GetCellAt(2, i).startY;
					MyUtils.ConsoleFunctions.WriteStringAtPosition(students[i].LastName, x, y);
				}
			}
		}

		private static void AddStudent()
		{
			Console.Clear();
			string fName = MyUtils.ConsoleFunctions.GetTextFromUser("Please enter the student's first name: ");
			string lName = MyUtils.ConsoleFunctions.GetTextFromUser("Please enter the student's last name: ");
			Student s = new Student()
			{
				FirstName = fName,
				LastName = lName
			};
			using (var db = new StudentDbContext())
			{
				db.Students.Add(s);
				db.SaveChanges();
			}
			Console.Clear();
		}

		private static void RemoveStudent()
		{
			using (var db = new StudentDbContext())
			{
				var query = from s in db.Students
							orderby s.Id
							select s;

				List<Student> students = query.ToList();

				if (students.Count > 0)
				{
					DisplayStudents();
					List<int> validIds = new List<int>();
					foreach( var student in students) validIds.Add(student.Id);
					while (true)
					{
						Console.WriteLine("Enter a student id to remove: ");
						int id = 0;
						bool success = Int32.TryParse(Console.ReadLine(), out id);
						if (success)
						{
							if (validIds.Contains(id))
							{
								int i = -1;
								for (i = 0; i < students.Count; i++) if (students[i].Id == id) break;
								if (i != -1)
								{
									db.Students.Remove(students[i]);
									db.SaveChanges();
									return;
								}
							}
							else
							{
								Console.WriteLine("Please enter a valid ID!");
								Console.WriteLine("Would you like to try again? (y/n)");
								if (!MyUtils.ConsoleFunctions.GetBoolFromUser()) break;
							}
						}
						else
						{
							Console.WriteLine("Please enter a valid ID!");
							Console.WriteLine("Would you like to try again? (y/n)");
							if (!MyUtils.ConsoleFunctions.GetBoolFromUser()) break;
						}
					}
				}
				else
				{
					Console.WriteLine("There are no students in the database!\nPress any key to return to main menu...");
					Console.ReadKey();
					Console.Clear();
				}
			}
		}
	}

	public class Student
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}

	public class StudentDbContext : DbContext
	{
		public DbSet<Student> Students { get; set; }
	}
}
