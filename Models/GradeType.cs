using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CampusManagementSystem.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GradeType
    {
        [EnumMember(Value = "A")]
        A,
        
        [EnumMember(Value = "B")]
        B,
        
        [EnumMember(Value = "C")]
        C,
        
        [EnumMember(Value = "D")]
        D,
        
        [EnumMember(Value = "F")]
        F,
        
        [EnumMember(Value = "I")]
        I,  // Incomplete
        
        [EnumMember(Value = "W")]
        W,  // Withdrawn
        
        [EnumMember(Value = "IP")]
        IP  // In Progress
    }
}