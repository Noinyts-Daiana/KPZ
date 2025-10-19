using AutoMapper;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows.Data;
using System.Windows.Input;

namespace KPZ_2
{
    public class FullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ApplicationModel app)
                return $"{app.Surname} {app.Name}";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object parameter) => _execute(parameter);
    public event EventHandler CanExecuteChanged;
}

public interface IDataService
{
    List<JobVacancy> LoadVacancies();
    void SaveApplications(List<JobApplication> applications);
}

public class FileDataService : IDataService
{
    public List<JobVacancy> LoadVacancies()
    {
        return DataSerializer.DeserializeData<JobVacancy>("vacancies.txt");
    }

    public void SaveApplications(List<JobApplication> applications)
    {
        DataSerializer.SerializeData("applications.txt", applications);
    }
}

public static class DataSerializer
{
    public static void SerializeData<T>(string fileName, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }

    public static List<T> DeserializeData<T>(string fileName)
    {
        if (!File.Exists(fileName))
            return new List<T>();

        var json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }
}
public static class MapperConfig
{
    private static IMapper _mapper;

    public static IMapper Initialize()
    {
        if (_mapper != null)
            return _mapper;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ApplicationModel, JobApplication>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname));

            cfg.CreateMap<JobApplication, ApplicationModel>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName));
        });

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
        return _mapper;
    }
}

