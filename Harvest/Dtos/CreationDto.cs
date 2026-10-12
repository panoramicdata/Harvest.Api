namespace Harvest.Dtos;

/// <summary>
/// The base of every request body sent to create a resource (<c>POST</c>). Required fields are C# <c>required</c>
/// members; optional fields are nullable and omitted from the JSON when <see langword="null"/>, so Harvest applies its own
/// defaults.
/// </summary>
public abstract class CreationDto
{
}