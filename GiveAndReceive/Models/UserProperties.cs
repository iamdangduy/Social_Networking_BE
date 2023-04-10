namespace GiveAndReceive.Models
{
    public class UserProperties
    {
        public string UserId { get; set; }
        public int RankId { get; set; }
        public string CitizenIdentificationName { get; set; }
        public string CitizenIdentificationNumber { get; set; }
        public string CitizenIdentificationPlaceOf { get; set; }
        public long CitizenIdentificationDateOf { get; set; }
        public string CitizenIdentificationAddress { get; set; }
        public string CitizenIdentificationImageFront { get; set; }
        public string CitizenIdentificationImageBack { get; set; }
        public string PhotoFace { get; set; }
        public bool IdentificationApprove { get; set; }
        public long TotalAmountGive { get; set; }
        public long TotalAmountReceive { get; set; }

        public class EnumIdentificationApprove
        {
            public const bool SYSTEM_DECLINE = false;
            public const bool SYSTEM_ACCEPT = true;
        }
    }
}
