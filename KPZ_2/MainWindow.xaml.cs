using AutoMapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace KPZ_2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IMapper _mapper;
        private readonly IDataService _dataService;

        public ObservableCollection<JobVacancy> Vacancies { get; set; } = new();
        public ObservableCollection<ApplicationModel> Applications { get; set; } = new();

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _surname;
        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); }
        }

        private string _jobTitle;
        public string JobTitle
        {
            get => _jobTitle;
            set { _jobTitle = value; OnPropertyChanged(); }
        }

        private JobVacancy _selectedVacancy;
        public JobVacancy SelectedVacancy
        {
            get => _selectedVacancy;
            set
            {
                _selectedVacancy = value;
                OnPropertyChanged();
                JobTitle = _selectedVacancy?.Title ?? "";
            }
        }

        public ICommand SubmitApplicationCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public MainViewModel() : this(new FileDataService())
        {
        }
        public MainViewModel(IDataService dataService)
        {
            _mapper = MapperConfig.Initialize();
            _dataService = dataService;

            LoadVacancies();

            SubmitApplicationCommand = new RelayCommand(_ => SubmitApplication());
            DeleteCommand = new RelayCommand(DeleteApplication);
            SaveCommand = new RelayCommand(_ => SaveApplications());
        }

        private void LoadVacancies()
        {
            Vacancies.Clear();
            var list = _dataService.LoadVacancies();
            foreach (var v in list)
                Vacancies.Add(v);
        }

        private void SubmitApplication()
        {
            var dto = new JobApplication
            {
                FirstName = Name,
                LastName = Surname,
                JobTitle = JobTitle
            };

            var model = _mapper.Map<ApplicationModel>(dto);
            Applications.Add(model);

            Name = Surname = JobTitle = "";
        }

        private void DeleteApplication(object obj)
        {
            if (obj is ApplicationModel app)
                Applications.Remove(app);
        }

        private void SaveApplications()
        {
            var dtoList = Applications
                .Select(app => _mapper.Map<JobApplication>(app))
                .ToList();

            _dataService.SaveApplications(dtoList);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}