namespace eClinicQueue.Data.Models;

public class Role
{
    public Role()
    {
        Name = string.Empty;
        Users = new List<User>();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}
