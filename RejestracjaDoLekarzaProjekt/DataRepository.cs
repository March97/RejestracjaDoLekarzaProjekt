using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RejestracjaDoLekarzaProjekt
{
    class DataRepository
    {
        DataContext db = new DataContext();

        public void AddPatient(string name, string surname, string pesel, string gender)
        {
            var newPerson = new Person(name, surname, pesel, gender);
            newPerson.Patients.Add(new Patient());
            db.Add(newPerson);
            db.SaveChanges();          
        }

        public Person GetPatient(string pesel)
        {
            return db.Persons
                .Include(x => x.Patients)
                .Where(x => x.Pesel == pesel).FirstOrDefault();
        }

        public List<Doctor> GetDoctorsBySpecjalizaton(int specjalizationId)
        {
            return db.Doctors
                .Where(x => x.SpecjalizationId == specjalizationId)
                .Include(x => x.Person)
                .Include(x => x.Specjalization)
                .ToList();
        }

        public Specjalization GetSpecjalization(string name)
        {
            return db.Specjalizations.Where(x => x.Name == name).FirstOrDefault();
        }

        public Person GetDoctorByName(string name, string surname)
        {
            if(name == "")
                return db.Persons
                    .Where(x => x.Surname == surname)
                    .Include(x => x.Doctors)
                    .FirstOrDefault();
            else
                return db.Persons
                    .Where(x => x.Surname == surname)
                    .Where(x => x.Name == name)
                    .Include(x => x.Doctors)
                    .FirstOrDefault();
        }

        public List<Visit> GetFreeVisitsForDoctor(int doctorId)
        {
            return db.Visits
                .Where(x => x.DoctorId == doctorId)
                .Where(x => x.PatientId == null)
                .ToList();
        }

        public void BookVisit(int visitId, int patientId)
        {
            var visit = db.Visits.Where(x => x.Id == visitId).First();
            visit.PatientId = patientId;
            db.SaveChanges();
        }

        public List<Specjalization> GetSpecjalizatons()
        {
            return db.Specjalizations
                .Include(x => x.Doctors)
                .ToList();
        }
    }
}
