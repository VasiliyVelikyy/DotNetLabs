using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfAppPraktika.Helper;
using WpfAppPraktika.Model;
using WpfAppPraktika.View;

namespace WpfAppPraktika.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {

       readonly string path = @"C:\Users\Vasiliy\source\repos\WpfAppPraktika_Json\WpfAppPraktika\DataModels\RoleData.json";

        string _jsonRoles = String.Empty;
        public string Error { get; set; }

        /// <summary>
        /// выбранная в списке должность
        /// </summary>
        private Role selectedRole;
        /// <summary>
        /// выбранная в списке должность
        /// </summary>
        public Role SelectedRole
        {
            get
            {
                return selectedRole;
            }
            set
            {
                selectedRole = value;
                OnPropertyChanged("SelectedRole");
                EditRole.CanExecute(true);
            }
        }
        /// <summary>
        /// коллекция должностей сотрудников
        /// </summary>
        public ObservableCollection<Role> ListRole { get; set; } = new
       ObservableCollection<Role>();

        public ObservableCollection<Role> LoadRole()
        {
            _jsonRoles = File.ReadAllText(path);
            if (_jsonRoles != null)
            {
                ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                return ListRole;
            }
            else
            {
                return null;
            }
        }

        public RoleViewModel()
        {
            // ListRole = LoadRole();
            ListRole = new ObservableCollection<Role>();

            // Загрузка данных по должностям сотрудников
            ListRole = GetRoles();
        }

        private ObservableCollection<Role> GetRoles()
        {
            using (var context = new CompanyEntities())
            {
                var query = from role in context.Roles
                            orderby role
                            select role;
                if (query.Count() != 0)
                {
                    foreach (var c in query)
                        ListRole.Add(c);
                }
            }
            return ListRole;
        }


        /// <summary>
        /// Нахождение максимального Id в коллекции
        /// </summary>
        /// <returns></returns>
        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                };
            }
            return max;
        }

        #region command AddRole
        /// команда добавления новой должности
        private RelayCommand addRole;

        public RelayCommand AddRole
        {
            get
            {
                return addRole ??
                (addRole = new RelayCommand(obj =>
                {
                Role newRole = new Role();
                WindowNewRole wnRole = new WindowNewRole
                {
                    Title = "Новая должность",
                    DataContext = newRole,
                };
                wnRole.ShowDialog();
                if (wnRole.DialogResult == true)
                {
                    using (var context = new CompanyEntities())
                    {
                        try
                        {
                            context.Roles.Add(newRole);
                                context.SaveChanges();
                                ListRole.Clear();
                                ListRole = GetRoles();
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

        #region Command EditRole
        private RelayCommand editRole;
        public RelayCommand EditRole
        {
            get
            {
                return editRole ??
                (editRole = new RelayCommand(obj =>
                {
                Role editRole = SelectedRole;
                WindowNewRole wnRole = new WindowNewRole
                {
                    Title = "Редактирование должности",
                    DataContext = editRole,
                };
                wnRole.ShowDialog();
                if (wnRole.DialogResult == true)
                {
                    using (var context = new CompanyEntities())
                        {
                            Role role = context.Roles.Find(editRole.Id);
                            if (role.NameRole != editRole.NameRole)
                                role.NameRole = editRole.NameRole.Trim();
                            try
                            {
                                context.SaveChanges();
                                ListRole.Clear();
                                ListRole = GetRoles();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" +
                               ex.Message, "Предупреждение");
                            }
                        }
                    }
                    else
                    {
                        ListRole.Clear();
                        ListRole = GetRoles();
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }
        #endregion

        #region DeleteRole
        /// команда добавления новой должности
        private RelayCommand _deleteRole;
        public RelayCommand DeleteRole
        {
            get
            {
                return _deleteRole ??
                (_deleteRole = new RelayCommand(obj =>
                {
                Role role = SelectedRole;
                using (var context = new CompanyEntities())
                {
                    // Поиск в контексте удаляемого автомобиля
                    Role delRole = context.Roles.Find(role.Id);
                    if (delRole != null)
                    {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по должности: " + delRole.NameRole,
                            
                             "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Roles.Remove(delRole);
                                    context.SaveChanges();
                                    ListRole.Remove(role);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" +
                                   ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedRole != null && ListRole.Count >
               0));
            }
        }
        #endregion



        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]
string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

