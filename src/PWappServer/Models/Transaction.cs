using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer.Models
{
    public class Transaction
    {

        public int Id { get; set; }

        public int Amount { get; set; }

        public DateTime Date { get; set; }

        /*
        public int ApplicationUserId { get; set; } // внешний ключ

        public ApplicationUser ApplicationUser { get; set; }  // навигационное свойство
        */

        /*
    public int ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public virtual ApplicationUser ApplicationUser { get; set; }

    public int ClientApplicationUserId { get; set; }
    [ForeignKey("ClientApplicationUserId")]
    public virtual ApplicationUser ClientApplicationUser { get; set; }
    */


        //Foreign key for Standard
        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }
        /*
        [JsonIgnore]
        [ForeignKey("SenderUsername")]
        public ApplicationUser Sender { get; set; }
        [JsonIgnore]
        [ForeignKey("RecipientUsername")]
        public ApplicationUser Recipient { get; set; }
        */


    }
}
