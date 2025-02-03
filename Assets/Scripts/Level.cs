using System.Runtime.Serialization;

[DataContract]
public class Level
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string MusicPath { get; set; }

    [DataMember]
    public int TotalJumps { get; set; }
    public int TotalAttempts { get; set; }
    public int KilledCount { get; set; }
}
