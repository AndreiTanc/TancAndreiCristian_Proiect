using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectDatabaseModel;
using System.Data.Entity;
using System.Data;

namespace TancAndreiCristian_Proiect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;

        ProjectDatabaseEntitiesModel ctx = new ProjectDatabaseEntitiesModel();

        CollectionViewSource employeesViewSource;
        CollectionViewSource tasksViewSource;
        CollectionViewSource assignmentsViewSource;


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            employeesViewSource = (CollectionViewSource)FindResource("employeeViewSource");
            employeesViewSource.Source = ctx.Employees.Local;

            tasksViewSource = (CollectionViewSource)FindResource("taskViewSource");
            tasksViewSource.Source = ctx.Tasks.Local;

            assignmentsViewSource = (CollectionViewSource)FindResource("employeeAssignmentsViewSource");
            //assignmentsViewSource.Source = ctx.Assignments.Local;
            
            ctx.Employees.Load();
            ctx.Tasks.Load();
            ctx.Assignments.Load();

            cmbEmployees.ItemsSource = ctx.Employees.Local;
            //cmbEmployees.DisplayMemberPath = "name";
            cmbEmployees.SelectedValuePath = "employeeId";

            cmbTasks.ItemsSource = ctx.Tasks.Local;
            cmbTasks.DisplayMemberPath = "description";
            cmbTasks.SelectedValuePath = "taskId";

            BindDataGrid();
        }


        private void BindDataGrid()
        {
            var queryOrder = from assignment in ctx.Assignments
                             join employee in ctx.Employees on assignment.employeeId equals
                             employee.employeeId
                             join task in ctx.Tasks on assignment.taskId
                 equals task.taskId
                             select new
                             {
                                 assignment.assignmentId,
                                 assignment.taskId,
                                 assignment.employeeId,
                                 employee.name,
                                 employee.position,
                                 task.description
                             };
            assignmentsViewSource.Source = queryOrder.ToList();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("now saving");
            Employee employee = null;
            if (action == ActionState.New)
            {
                MessageBox.Show("new");
                try
                {
                    //instantiem Customer entity
                    employee = new Employee()
                    {
                        name = nameTextBox.Text.Trim(),
                        position = positionTextBox.Text.Trim(),
                        experienceInYears = float.Parse(experienceInYearsTextBox.Text.Trim())
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Employees.Add(employee);
                    employeesViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                MessageBox.Show("edit");
                try
                {
                    employee = (Employee)employeeDataGrid.SelectedItem;
                    employee.name = nameTextBox.Text.Trim();
                    employee.position = positionTextBox.Text.Trim();
                    employee.experienceInYears = int.Parse(experienceInYearsTextBox.Text.Trim());

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                employeesViewSource.View.Refresh();

                // pozitionarea pe item-ul curent
                employeesViewSource.View.MoveCurrentTo(employee);
            }
            else if (action == ActionState.Delete)
            {
                MessageBox.Show("delete");
                try
                {
                    employee = (Employee)employeeDataGrid.SelectedItem;
                    ctx.Employees.Remove(employee);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                employeesViewSource.View.Refresh();

            } 
            action = ActionState.Nothing;
        }


        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("now saving");
            ProjectDatabaseModel.Task task = null;
            if (action == ActionState.New)
            {
                MessageBox.Show("New");
                try
                {
                    //instantiem Inventory entity
                    task = new ProjectDatabaseModel.Task()
                    {
                        description = descriptionTextBox.Text.Trim(),
                        timeRemaining = float.Parse(timeRemainingTextBox.Text.Trim())
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Tasks.Add(task);
                    tasksViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                MessageBox.Show("Edit");
                try
                {
                    task = (ProjectDatabaseModel.Task)taskDataGrid.SelectedItem;
                    task.description = descriptionTextBox.Text.Trim();
                    task.timeRemaining = float.Parse(timeRemainingTextBox.Text.Trim());

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                tasksViewSource.View.Refresh();

                tasksViewSource.View.MoveCurrentTo(task);
                // pozitionarea pe item-ul curent
            }
            else if (action == ActionState.Delete)
            {
                MessageBox.Show("Delete");
                try
                {
                    task = (ProjectDatabaseModel.Task)taskDataGrid.SelectedItem;
                    ctx.Tasks.Remove(task);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                tasksViewSource.View.Refresh();
            }

            action = ActionState.Nothing;
        }


        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Assignment assignment = null;
            if (action == ActionState.New)
            {
                try
                {
                    Employee employee = (Employee)cmbEmployees.SelectedItem;
                    ProjectDatabaseModel.Task task = (ProjectDatabaseModel.Task)cmbTasks.SelectedItem;
                    //instantiem Order entity
                    assignment = new Assignment()
                    {
                        employeeId = employee.employeeId,
                        taskId = task.taskId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Assignments.Add(assignment);
                    assignmentsViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                assignmentsViewSource.View.Refresh();

            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedAssignment = assignmentsDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedAssignment.assignmentId;
                    var editedAssignment = ctx.Assignments.FirstOrDefault(s => s.assignmentId == curr_id);
                    if (editedAssignment != null)
                    {
                        editedAssignment.employeeId = Int32.Parse(cmbEmployees.SelectedValue.ToString());
                        editedAssignment.taskId = Convert.ToInt32(cmbTasks.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                employeesViewSource.View.MoveCurrentTo(selectedAssignment);

                assignmentsViewSource.View.Refresh();
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedAssignment = assignmentsDataGrid.SelectedItem;
                    int curr_id = selectedAssignment.assignmentId;
                    var deletedAssignment = ctx.Assignments.FirstOrDefault(s => s.assignmentId == curr_id);
                    if (deletedAssignment != null)
                    {
                        ctx.Assignments.Remove(deletedAssignment);
                        ctx.SaveChanges();
                        MessageBox.Show("Assignment Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            action = ActionState.Nothing;
        }

            
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            BindingOperations.ClearBinding(experienceInYearsTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(positionTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            employeeIdTextBox.Text = "";
            nameTextBox.Text = "";
            positionTextBox.Text = "";
            experienceInYearsTextBox.Text = "";

            descriptionTextBox.Text = "";
            taskIdTextBox.Text = "";
            timeRemainingTextBox.Text = "";


        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            employeesViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            employeesViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            tasksViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            tasksViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            assignmentsViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious2_Click(object sender, RoutedEventArgs e)
        {
            assignmentsViewSource.View.MoveCurrentToPrevious();
        }


        private void SetValidationBinding()
        {
            Binding employeesExperienceValidationBinding = new Binding();
            employeesExperienceValidationBinding.Source = employeesViewSource;
            employeesExperienceValidationBinding.Path = new PropertyPath("experienceInYears");
            employeesExperienceValidationBinding.NotifyOnValidationError = true;
            employeesExperienceValidationBinding.Mode = BindingMode.TwoWay;
            employeesExperienceValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            employeesExperienceValidationBinding.ValidationRules.Add(new StringNotEmpty());
            experienceInYearsTextBox.SetBinding(TextBox.TextProperty, employeesExperienceValidationBinding);


            Binding employeesNameValidationBinding = new Binding();
            employeesNameValidationBinding.Source = employeesViewSource;
            employeesNameValidationBinding.Path = new PropertyPath("name");
            employeesNameValidationBinding.NotifyOnValidationError = true;
            employeesNameValidationBinding.Mode = BindingMode.TwoWay;
            employeesNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            employeesNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            nameTextBox.SetBinding(TextBox.TextProperty, employeesNameValidationBinding); //setare binding nou

            Binding employeesPositionValidationBinding = new Binding();
            employeesPositionValidationBinding.Source = employeesViewSource;
            employeesPositionValidationBinding.Path = new PropertyPath("position");
            employeesPositionValidationBinding.NotifyOnValidationError = true;
            employeesPositionValidationBinding.Mode = BindingMode.TwoWay;
            employeesPositionValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            employeesPositionValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            positionTextBox.SetBinding(TextBox.TextProperty, employeesPositionValidationBinding); //setare binding nou
        }

    }


    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
}
