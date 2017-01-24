using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PWappServer.Models;
using System.ComponentModel.DataAnnotations.Schema;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
  
    /// <summary>Surname(s) or last name(s) of the End-User.</summary>
    public virtual string Name { get; set; }

    public virtual int PW { get; set; }



    /*
    public virtual List<Transaction> Transactions { get; set; }

    public virtual List<Transaction> OutTransactions { get; set; }
    */

    /*
public virtual ICollection<Transaction> Transactions { get; set; }

public virtual ICollection<Transaction> OutTransactions { get; set; }

*/
/*
    [InverseProperty("Sender")]
    public IList<Transaction> Transactions { get; set; }

    [InverseProperty("Recipient")]
    public IList<Transaction> Transaction2 { get; set; }
*/

}