using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using System.Windows;
using WpfAppPraktika.Helper;
using WpfAppPraktika.Model;
using WpfAppPraktika.View;

namespace WpfAppPraktika.ViewModel
{
    

    public class PersonViewModel : INotifyPropertyChanged
    {
        readonly string path = @"C:\Users\Vasiliy\source\repos\WpfAppPraktika_Json\WpfAppPraktika\DataModels\PersonData.json";
      
        string _jsonPersons = String.Empty;

        public string Error { get; set; }
        public string Message { get; set; }


        private Person selectedPerson;
        /// <summary>
        /// выделенные в списке данные по сотруднику 
        /// </summary>
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged("SelectedPersonDpo");
            }
        }


        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPerson = GetPersons();
        }

        private ObservableCollection<Person> GetPersons()
        {
            using (var context = new CompanyEntities())
            {
                var query = from per in context.Persons
                .Include("Role")
                            orderby per.LastName
                            select per;
                if (query.Count() != 0)
                {
                    foreach (var p in query)
                    {
                        ListPerson.Add(p);
                    }
                }
            }
            return ListPerson;
        }



        /// <summary>
        /// коллекция данных по сотрудникам
        /// </summary>
        public ObservableCollection<Person> ListPerson { get; set; } =
       new ObservableCollection<Person>();

        public ObservableCollection<Person> LoadPerson()
        {
            _jsonPersons = File.ReadAllText(path);
            if (_jsonPersons != null)
            {
                ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
                return ListPerson;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Нахождение максимального Id в коллекции данных
        /// </summary>
        /// <returns></returns>
        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListPerson)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                };
            }
            return max;
        }

        #region AddPerson
        /// <summary>
        /// добавление сотрудника
        /// </summary>
        private RelayCommand _addPerson;
        /// <summary>
        /// добавление сотрудника
        /// </summary>
        public RelayCommand AddPerson
        {
            get
            {
                return _addPerson ??
                (_addPerson = new RelayCommand(obj =>
                {
                Person newPerson = new Person
                {
                    Birthday = DateTime.Now
                };
                WindowNewEmployee wnPerson = new WindowNewEmployee
                {
                    Title = "Новый сотрудник",
                    DataContext = newPerson
                };
                wnPerson.ShowDialog();
                if (wnPerson.DialogResult == true)
                {
                    using (var context = new CompanyEntities())
                    {
                        try
                            {
                                Person ord = context.Persons.Add(newPerson);
                                context.SaveChanges();
                                ListPerson.Clear();
                                ListPerson = GetPersons();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка добавления данных!\n" +
                               ex.Message, "Предупреждение");
                            }
                        }
                    }
                }, (obj) => true));
            }
        }
        #endregion


        #region EditPerson
        /// команда редактирования данных по сотруднику
        private RelayCommand _editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return _editPerson ??
                (_editPerson = new RelayCommand(obj =>
                {
                Person editPerson = SelectedPerson;
                    WindowNewEmployee wnPerson = new WindowNewEmployee()
                    {
                        Title = "Редактирование данных сотрудника",
                        DataContext = editPerson
                    };
                    wnPerson.ShowDialog();
                    if (wnPerson.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Person person = context.Persons.Find(editPerson.Id);
                            if (person != null)
                            {
                                if (person.RoleId != editPerson.RoleId)
                                    person.RoleId = editPerson.RoleId;
                                if (person.FirstName != editPerson.FirstName)
                                    person.FirstName = editPerson.FirstName;
                                if (person.LastName != editPerson.LastName)
                                    person.LastName = editPerson.LastName;
                                if (person.Birthday != editPerson.Birthday)
                                    person.Birthday = editPerson.Birthday;
                                try
                                {
                                    context.SaveChanges();
                                    ListPerson.Clear();
                                    ListPerson = GetPersons();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка редактирования данных!\n"
                                   + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                    else
                    {
                        ListPerson.Clear();
                        ListPerson = GetPersons();
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count >
               0));
            }
        }
        #endregion

        #region DeletePerson
        /// команда удаления данных по сотруднику
        private RelayCommand _deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return _deletePerson ??
                (_deletePerson = new RelayCommand(obj =>
                {
                    Person delPerson = SelectedPerson;
                    using (var context = new CompanyEntities())
                    {
                        // Поиск в контексте удаляемого автомобиля
                        Person person = context.Persons.Find(delPerson.Id);
                        if (person != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \nФамилия: " + person.LastName + 
                    "\nИмя: " + person.FirstName, "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Persons.Remove(person);
                                    context.SaveChanges();
                                    ListPerson.Remove(delPerson);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" +
                    ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count >
               0));
            }
        }
        #endregion

        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            var jsonPerson = JsonConvert.SerializeObject(listPersons);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonPerson);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
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


