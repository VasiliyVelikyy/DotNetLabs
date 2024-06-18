using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfAppPraktika.ViewModel;

namespace WpfAppPraktika.Model
{
    /// <summary>
    /// класс отображения данных по сотруднику
    /// </summary>
    public class PersonDPO : INotifyPropertyChanged
    {
        /// <summary>
        /// код сотрудника
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// должность сотрудника
        /// </summary>
        private string _roleName;
        /// <summary>
        /// должность сотрудника
        /// </summary>
        public string RoleName
        {
            get { return _roleName; }
            set
            {
                _roleName = value;
                OnPropertyChanged("RoleName");
            }
        }
        /// <summary>
        /// имя сотрудника
        /// </summary>
        private string firstName;
        /// <summary>
        /// имя сотрудника
        /// </summary>
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        /// <summary>
        /// фамилия сотрудника
        /// </summary>
        private string lastName;
        /// <summary>
        /// фамилия сотрудника
        /// </summary>
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        /// <summary>
        /// дата рождения сотрудника
        /// </summary>
        private string birthday;
        /// <summary>
        /// дата рождения сотрудника
        /// </summary>
        public string Birthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                OnPropertyChanged("Birthday");
            }
        }
        public PersonDPO() { }
        public PersonDPO(int id, string roleName, string firstName,
       string lastName, string birthday)
        {
            this.Id = id;
            this.RoleName = roleName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Birthday = birthday;
        }
        /// <summary>
        /// Метод поверхностного копирования 
        /// </summary>
        /// <returns></returns>
        public PersonDPO ShallowCopy()
        {
            return (PersonDPO)this.MemberwiseClone();
        }
        /// <summary>
        /// копирование данных из класса Person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public PersonDPO CopyFromPerson(Person person)
        {
            PersonDPO perDpo = new PersonDPO();
            RoleViewModel vmRole = new RoleViewModel();
            string role = string.Empty;
            foreach (var r in vmRole.ListRole)
            {
                if (r.Id == person.RoleId)
                {
                    role = r.NameRole;
                    break;
                }
            }
            if (role != string.Empty)
            {
                perDpo.Id = person.Id;
                perDpo.RoleName = role;
                perDpo.FirstName = person.FirstName;
                perDpo.LastName = person.LastName;
                perDpo.Birthday = person.Birthday;
            }
            return perDpo;
        }

        public static  string GetStringBirthday(string birthday)
        {
            if (birthday.Contains("/"))
            {
                var end = birthday.IndexOf(" ");
                birthday = birthday.Substring(0, end);
                birthday = birthday.Replace("/", ".");
            }
            else if (birthday.Contains(":")) {
                var end = birthday.IndexOf(" ");
                birthday = birthday.Substring(0, end);
               
            }
            return birthday;
        }

        public event PropertyChangedEventHandler PropertyChanged;
       //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]
        string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


