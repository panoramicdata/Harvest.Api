namespace Harvest.Dtos;

/// <summary>
/// The base of every request body sent to update a resource (<c>PATCH</c>). Every field is nullable and omitted from the
/// JSON when <see langword="null"/>, so Harvest leaves it unchanged.
/// </summary>
public abstract class PatchDto
{
}