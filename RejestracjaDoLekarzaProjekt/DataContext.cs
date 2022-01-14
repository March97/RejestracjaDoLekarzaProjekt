using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace RejestracjaDoLekarzaProjekt
{
    class DataContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Specjalization> Specjalizations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Visit> Visits { get; set; }

        public string DbPath { get; }

        public DataContext()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            DbPath = projectDirectory + "\\clinic.db";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class Person
    {
        public Person(string name, string surname, string pesel, string gender)
        {
            Name = name;
            Surname = surname;
            Pesel = pesel;
            Gender = gender;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pesel { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }

        public List<Patient> Patients { get; } = new List<Patient>();
        public List<Doctor> Doctors { get; } = new List<Doctor>();
    }

    public class Specjalization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Doctor> Doctors { get; } = new List<Doctor>();
    }

    public class Patient
    {
        public int Id { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }

        public List<Visit> Visits { get; } = new List<Visit>();
    }

    public class Doctor
    {
        public int Id { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }

        public int SpecjalizationId { get; set; }
        public Specjalization Specjalization { get; set; }

        public List<Visit> Visits { get; } = new List<Visit>();
    }

    public class Visit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
