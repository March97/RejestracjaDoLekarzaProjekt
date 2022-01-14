using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
