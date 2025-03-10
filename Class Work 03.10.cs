using System.Text;
using System.Threading;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Game;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Mysqlx.Datatypes;
using System.Xml.Linq;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using Org.BouncyCastle.Asn1.Cms;

namespace SnakeGame
{

    class DBManager
    {
        MySqlConnection connection;

        public DBManager(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void CreateTables(string query)
        {
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void AddDepartment(string name)
        {
            string query =
                "INSERT INTO Departments(Name) Value(@name)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }

        }

        public void AddSpecialization(string name)
        {
            string query =
                "INSERT INTO Specializations (Name) VALUES (@name)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }

        }

        public void AddDoctorSpecialization(int DoctorID, int SpecializationID)
        {
            string query =
                "INSERT INTO doctorspecializations (DoctorID, SpecializationID) VALUES (@doctorID, @specializationID)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("doctorID", DoctorID);
                command.Parameters.AddWithValue("specializationID", SpecializationID);
                command.ExecuteNonQuery();
            }

        }


        public void AddDoctor(string name, decimal premium, decimal salary, int departmentID)
        {
            string query =
                "INSERT INTO Doctors(Name, Premium, Salary, DepartmentID) Value(@name, @premium, @salary, @departmentID)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@premium", premium);
                command.Parameters.AddWithValue("@salary", salary);
                command.Parameters.AddWithValue("@departmentID", departmentID);
                command.ExecuteNonQuery();
            }

        }

        public void RemoveDoctor(int id)
        {
            string query =
                "DELETE FROM Doctors WHERE ID=@id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

        }

        public void RemoveSpecializations(int id)
        {
            string query =
                "DELETE FROM Specializations WHERE ID=@id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

        }

        public void RemoveDoctorSpecializations(int id)
        {
            string query =
                "DELETE FROM DoctorSpecializations WHERE ID=@id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

        }
        


        public void EditDoctor(int Id, Doctors doctor)
        {
            string query =
                "UPDATE Doctors SET ID=@id, Name=@name, Premium=@premium, Salary=@salary, DepartmentID=@departmentID WHERE ID=@old_Id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@old_Id", Id);
                command.Parameters.AddWithValue("@id", doctor.ID);
                command.Parameters.AddWithValue("@name", doctor.Name);
                command.Parameters.AddWithValue("@premium", doctor.Premium);
                command.Parameters.AddWithValue("@salary", doctor.Salary);
                command.Parameters.AddWithValue("@departmentID", doctor.DepartmentID);
                command.ExecuteNonQuery();
            }

        }



        public List<Doctors> GetDoctors()
        {
            List<Doctors> doctor = new List<Doctors>();
            List<DoctorsSpecialization> specializations = new List<DoctorsSpecialization>();
            List<Specialization> specialization = new List<Specialization>();
            specializations = GetDoctorSpecializations();
            specialization = GetSpecializations();

            string query = "SELECT * FROM Doctors";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    doctor.Add(new Doctors((int)reader["ID"], (string)reader["Name"], (Decimal)reader["Premium"], (Decimal)reader["Salary"], (int)reader["DepartmentID"]));
                    for (int i = 0; i < specializations.Count(); i++)
                        if (specializations.Count() != 0 && doctor.Last().ID == specializations[i].DoctorID)
                            for (int j = 0; j < specialization.Count(); j++)
                                if (specializations[i].SpecializationID == specialization[j].ID)
                                    doctor.Last().addSpecialization(specialization[j]);
                }
                return doctor;
            }
        }

        public List<Department> GetDepartments()
        {
            List<Department> Departments = new List<Department>();
            string query = "SELECT * FROM Departments";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    Departments.Add(new Department((int)reader["ID"], (string)reader["Name"]));
                }
                return Departments;
            }
        }

        public List<Specialization> GetSpecializations()
        {
            List<Specialization> Specializations = new List<Specialization>();
            string query = "SELECT * FROM Specializations";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    Specializations.Add(new Specialization((int)reader["ID"], (string)reader["Name"]));
                }
                return Specializations;
            }
        }
        public List<DoctorsSpecialization> GetDoctorSpecializations()
        {
            List<DoctorsSpecialization> DoctorsSpecializations = new List<DoctorsSpecialization>();
            string query = "SELECT * FROM Doctorspecializations";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    DoctorsSpecializations.Add(new DoctorsSpecialization((int)reader["ID"], (int)reader["DoctorID"], (int)reader["SpecializationID"]));
                }
                return DoctorsSpecializations;
            }
        }

        public void ShowDepartments(List<Department> Departments)
        {
            Console.WriteLine($"{"ID",3} | {"Name",20}");

            foreach (Department dep in Departments)
            {
                Console.WriteLine(dep);
            }
        }
        public void ShowDoctors(List<Doctors> doctors)
        {
            Console.WriteLine($"{"ID",3} | {"Name",20} | {"Salary",13} | {"Premium",12} | {"Department",20}");

            foreach (Doctors doc in doctors)
            {
                Console.WriteLine(doc);
            }
        }
        public void ShowDoctorSpecialization(Doctors doctors)
        {
            
            Console.Write($"{doctors.ID,3} | {doctors.Name,20} | ");
            foreach (Specialization spec in doctors.specializations)
            {
                Console.Write(spec + " | ");
            }
            Console.WriteLine();
        }
        public void ShowDoctorsAndDepartment(List<Doctors> doctors, List<Department> Departments)
        {
            string Name = "";
            Console.WriteLine($"{"ID",3} | {"Name",20} | {"Salary",13} | {"Premium",12} | {"Department",20}");

            foreach (Doctors doc in doctors)
            {
                for (int i = 0; i < Departments.Count(); i++)
                    if (doc.DepartmentID == Departments[i].ID)
                        Name = Departments[i].Name;

                Console.WriteLine($"{doc.ID,3} | {doc.Name,20} | {doc.Premium,8} грн. | {doc.Salary,7} грн. | {Name,20}");
            }
        }

        public void IntiateANewHospitale()
        {
            List<string> Name = new List<string> {
                "Denis", "Misha", "HoWL", "None", "Sasha", "Ivan", "Petro"
            };
            List<string> Surname = new List<string> {
                "Jordan", "Ivanow", "Petrov", "None", "Sidorenko"
            };
            List<Decimal> Salary = new List<Decimal> {
                1000, 1200, 1400, 1300, 1700, 2000, 1800, 4000, 2100
            };
            List<Decimal> Premium = new List<Decimal> {
                100, 120, 140, 130, 170, 200, 180, 400, 210, 300, 500, 1000, 700
            };
            List<string> DepartmentNames = new List<string> {
                "Терапія", "Кардіологія", "Травматологія"
            };
            CreateTables(
                "CREATE TABLE IF NOT EXISTS Departments (" +
                "  ID INT AUTO_INCREMENT KEY," +
                "  Name VARCHAR(100)" +
                ");");
            CreateTables(
                "CREATE TABLE IF NOT EXISTS Doctors(" +
                "  ID INT AUTO_INCREMENT PRIMARY KEY," +
                "  Name VARCHAR(100)," +
                "  Salary DECIMAL," +
                "  Premium DECIMAL," +
                "  DepartmentID INT," +
                "  FOREIGN KEY (DepartmentID) REFERENCES Departments(ID)" +
                ");");
            CreateTables(
                "CREATE TABLE IF NOT EXISTS Specializations (" +
                "   ID INT AUTO_INCREMENT KEY," +
                "   Name VARCHAR(100)" +
                ");"
                );
            CreateTables(
                "CREATE TABLE IF NOT EXISTS DoctorSpecializations (" +
                "  ID INT AUTO_INCREMENT KEY," +
                "  DoctorID INT," +
                "  SpecializationID INT," +
                "  FOREIGN KEY (SpecializationID) REFERENCES Specializations(ID)," +
                "  FOREIGN KEY (DoctorID) REFERENCES Doctors(ID)" +
                ");");




            Random rnd = new Random();
            if (GetDepartments().Count == 0)
                for (int i = 0; i < DepartmentNames.Count(); i++)
                    AddDepartment(DepartmentNames[i]);
            if (GetDoctors().Count() == 0)
                for (int i = 0; i < 6; i++)
                {
                    AddDoctor($"{Name[rnd.Next(0, Name.Count())]} {Surname[rnd.Next(0, Surname.Count())]}", Premium[rnd.Next(0, Premium.Count())], Salary[rnd.Next(0, Salary.Count())], rnd.Next(1, DepartmentNames.Count() + 1));
                }
            if (GetSpecializations().Count() == 0) {
                AddSpecialization("Кардиолог");
                AddSpecialization("Хирург");
                AddSpecialization("Педиатр");
            }
            var list = GetSpecializations();
            List<int> list_id = new List<int>();
            for (int i = 0; i < list.Count(); i++)
            {
                list_id.Add(list[i].ID);
            }
            if (GetDoctorSpecializations().Count() == 0 )
            {   
                var doctors = GetDoctors();
                for(int i = 0; i < doctors.Count(); i++)
                {
                    for (int j = 0; j < rnd.Next(1, 4); i++) {
                        AddDoctorSpecialization(doctors[i].ID, list_id[rnd.Next(0, list_id.Count() - 1)]);
                    }
                }
            }
        }
    }
    class Specialization
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Specialization(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
        public override string ToString()
        {
            return $"{Name,20}";
        }
    }
    class DoctorsSpecialization
    {
        public int ID { get; set; }
        public int DoctorID { get; set; }
        public int SpecializationID { get; set; }
        public DoctorsSpecialization(int ID, int DoctorID, int SpecializationID)
        {
            this.ID = ID;
            this.DoctorID = DoctorID;
            this.SpecializationID = SpecializationID;
        }
        public override string ToString()
        {
            return $"{ID,3} | {DoctorID,12} | {SpecializationID,20}";
        }
    }

    class Department
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Department(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
        public override string ToString()
        {
            return $"{ID,3} | {Name,20}";
        }
    }

    class Doctors
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Decimal Salary { get; set; }
        public Decimal Premium { get; set; }
        public int DepartmentID { get; set; }
        public List<Specialization> specializations = new List<Specialization>();

        public Doctors(int ID, string Name, Decimal Salary, Decimal Premium, int DepartmentID)
        {
            this.ID = ID;
            this.Name = Name;
            this.Salary = Salary;
            this.Premium = Premium;
            this.DepartmentID = DepartmentID;
        }
        public override string ToString()
        {
            return $"{ID,3} | {Name,20} | {Premium,8} грн. | {Salary,7} грн. | {DepartmentID,20}";
        }
        public void addSpecialization(Specialization spec)
        {
            specializations.Add(spec);
        }

    }

    class Program
    {

        public static void Main(string[] args)
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            Console.InputEncoding = UTF8Encoding.UTF8;
            string db_host = "localhost";
            string db_database = "testhospital";
            string db_user = "root";
            string db_password = "";
            ConsoleKey key;

            string connectionString = $"Server={db_host};Database={db_database};User ID={db_user};Passwork={db_password};";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    DBManager DB = new DBManager(connection);
                    connection.Open();

                    DB.IntiateANewHospitale();
                    connection.Close();
                    connection.Open();
                    var doc = DB.GetDoctors();
                    var depart = DB.GetDepartments();
                    DB.ShowDoctorsAndDepartment(doc, depart);
                    Console.WriteLine();
                    Console.WriteLine($"{"ID",3} | {"Name",20} | {"Specialization",20} | ");
                    foreach (var doctor in doc)
                    {
                        DB.ShowDoctorSpecialization(doctor);
                    }

                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

        }
    }
}
