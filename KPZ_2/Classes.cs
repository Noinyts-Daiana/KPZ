
using System.ComponentModel;
using System.Runtime.CompilerServices;

[Serializable]
public class JobVacancy
{
    public string Title { get; set; }
    public string Specialization { get; set; }
}

[Serializable]
public class JobApplication 
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
}

public class ApplicationModel 
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string JobTitle { get; set; }
}
