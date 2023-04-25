using System.Text.Json.Serialization;
using JobOdysseyApi.Validations;

namespace JobOdysseyApi.Dtos;

public class UpdateJobApplicationStatusDto
{
    [JsonConverter(typeof(NullableIntConverter))]
    [ApplicationStatusValidation(allowNull: false)]
    public int? Status { get; set; }
}