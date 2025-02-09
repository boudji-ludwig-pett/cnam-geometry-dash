using System.Runtime.Serialization;

[DataContract]
public class Level
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string MusicName { get; set; }

    [DataMember]
    public int TotalJumps { get; set; }

    [DataMember]
    public int TotalAttempts { get; set; }

    [DataMember]
    public int KilledCount { get; set; }
}
