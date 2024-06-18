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
using Hangfire.Annotations;
using System.Data;
using System.Windows.Controls;

namespace WpfAppPraktika.ViewModel
{
    

    public class PersonViewModel : INotifyPropertyChanged
    {
        readonly string path = @"C:\Users\Vasiliy\source\repos\Labs\WpfAppPraktika_Json\WpfAppPraktika\DataModels\PersonData.json";
      
        string _jsonPersons = String.Empty;

        public string Error { get; set; }
        public string Message { get; set; }

        private PersonDPO selectedPersonDpo;
        /// <summary>
        /// выделенные в списке данные по сотруднику 
        /// </summary>
        public PersonDPO SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
            }
        }

    

        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPersonDpo = new ObservableCollection<PersonDPO>();
            ListPerson = LoadPerson();
            ListPersonDpo = GetListPersonDpo();
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

        public ObservableCollection<PersonDPO> ListPersonDpo
        {
            get;
            set;
        } = new ObservableCollection<PersonDPO>();
       
        public ObservableCollection<PersonDPO> GetListPersonDpo()
        {
            foreach (var person in ListPerson)
            {
                PersonDPO p = new PersonDPO();
                p = p.CopyFromPerson(person);
                ListPersonDpo.Add(p);
            }
            return ListPersonDpo;
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
        private RelayCommand addPerson;
        /// <summary>
        /// добавление сотрудника
        /// </summary>
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                (addPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник"
                    };
                    // формирование кода нового собрудника
                    int maxIdPerson = MaxId() + 1;
                    PersonDPO per = new PersonDPO
                    {
                        Id = maxIdPerson,
                      //  Birthday = DateTime.Now.ToString("dd.mmmmm.yyyy")
                        Birthday = DateTime.Now.ToString()
                    };
                    wnPerson.DataContext = per;

                    wnPerson.CbRole.ItemsSource = new RoleViewModel().ListRole;
                    if (wnPerson.ShowDialog() == true)
                    {

                        var r = (Role)wnPerson.CbRole.SelectedValue;
                        if (r != null)
                        {
                            per.RoleName = r.NameRole;
                            per.Birthday = PersonDPO.GetStringBirthday(per.Birthday);
                            ListPersonDpo.Add(per);
                            // добавление нового сотрудника в коллекцию ListPerson<Person> 
                            Person p = new Person();
                            p = p.CopyFromPersonDPO(per);
                            ListPerson.Add(p);

                            try
                            {
                                // сохранение изменений в файле json
                                SaveChanges(ListPerson);
                            }
                            catch (Exception e)
                            {
                                Error = "Ошибка добавления данных в json файл\n" +
                               e.Message;
                            }
                        }
                        else {
                            MessageBoxResult result = MessageBox.Show("Не выбрана роль: \n" ,
"Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    SelectedPersonDpo = per;
                },
 (obj) => true));
            }
        }


        #endregion
        #region EditPerson
        /// команда редактирования данных по сотруднику
        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                (editPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee()
                    {
                        Title = "Редактирование данных сотрудника",
                    };
                    PersonDPO personDpo = SelectedPersonDpo;
                    PersonDPO tempPerson = new PersonDPO();
                    tempPerson = personDpo.ShallowCopy();
                    wnPerson.DataContext = tempPerson;
               
                    wnPerson.CbRole.ItemsSource = new RoleViewModel().ListRole;

                    if (wnPerson.ShowDialog() == true)
                    {
                        // сохранение данных в оперативной памяти
                        // перенос данных из временного класса в класс отображения данных 
                        var r = (Role)wnPerson.CbRole.SelectedValue;
                        if (r != null)
                        {
                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            personDpo.Birthday = PersonDPO.GetStringBirthday(tempPerson.Birthday);
                            // перенос данных из класса отображения данных в  класс Person
                        var per = ListPerson.FirstOrDefault(p => p.Id == personDpo.Id);
                            if (per != null)
                            {
                                per = per.CopyFromPersonDPO(personDpo);
                            }
                            try
                            {
                                // сохраненее данных в файле json
                                SaveChanges(ListPerson);
                            }
                            catch (Exception e)
                            {
                                Error = "Ошибка редактирования данных в json файл\n"+ e.Message;
                            }
                        }
                        else
                        {
                            Message = "Необходимо выбрать должность сотрудника.";
                        }                  
                    }
                }, (obj) =>  ListPersonDpo.Count > 0)); //SelectedPersonDpo != null && ListPersonDpo.Count > 0)

            }
        }



        #endregion
        #region DeletePerson
        /// команда удаления данных по сотруднику
        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                   
                        selectedPersonDpo = ListPersonDpo.Last();
                    
                    PersonDPO person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
 "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        try
                        {
                            // удаление данных в списке отображения данных
                            ListPersonDpo.Remove(person);
                            // поиск удаляемого класса в коллекции ListRoles
                            var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);
                            if (per != null)
                            {
                                ListPerson.Remove(per);
                                // сохраненее данных в файле json
                                SaveChanges(ListPerson);
                            }
                        }
                        catch (Exception e)
                        {
                            Error = "Ошибка удаления данных\n" + e.Message;
                        }
                  
                    }
                }, (obj) =>  ListPersonDpo.Count > 0)); //SelectedPersonDpo != null && ListPersonDpo.Count > 0)
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
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]
string propertyName = "")
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


