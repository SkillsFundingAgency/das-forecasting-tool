using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.EmployerUsers;

public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class EmployerAccountService : IEmployerAccountService
{
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        throw new System.NotImplementedException();
    }
}
//TODO FAI-656

public class EmployerUserAccounts
{
    public string Email { get; set; }
    public string EmployerUserId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public IEnumerable<EmployerUserAccountItem> EmployerAccounts { get ; set ; }

    public static implicit operator EmployerUserAccounts(GetUserAccountsResponse source)
    {
        if (source?.UserAccounts == null)
        {
            return new EmployerUserAccounts
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                EmployerUserId = source.EmployerUserId,
                Email = source.Email,
                EmployerAccounts = new List<EmployerUserAccountItem>()
            };
        }
            
        return new EmployerUserAccounts
        {
            EmployerAccounts = source.UserAccounts.Select(c=>(EmployerUserAccountItem)c).ToList()
        };
    }
}

public class EmployerUserAccountItem
{
    public string AccountId { get; set; }
    public string EmployerName { get; set; }
    public string Role { get; set; }
        
    public static implicit operator EmployerUserAccountItem(EmployerIdentifier source)
    {
        return new EmployerUserAccountItem
        {
            AccountId = source.AccountId,
            EmployerName = source.EmployerName,
            Role = source.Role
        };
    }
}


public class GetUserAccountsResponse
{
    [JsonProperty]
    public string EmployerUserId { get; set; }
    [JsonProperty]
    public string FirstName { get; set; }
    [JsonProperty]
    public string LastName { get; set; }
    [JsonProperty]
    public string Email { get; set; }
    [JsonProperty("UserAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}
    
public class EmployerIdentifier
{
    [JsonProperty("EncodedAccountId")]
    public string AccountId { get; set; }
    [JsonProperty("DasAccountName")]
    public string EmployerName { get; set; }
    [JsonProperty("Role")]
    public string Role { get; set; }
}