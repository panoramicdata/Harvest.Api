namespace Harvest.Models;

/// <summary>
/// A user's capacity, rates, roles and permissions profile, as a <see cref="User"/> reports them and as the bodies that
/// create and update a user set them.
/// </summary>
public abstract class UserSettings
{
	/// <summary>
	/// Whether the user can sign in without single sign-on when SAML is enabled for the account; on a new user Harvest's
	/// default is <see langword="false"/>.
	/// </summary>
	public bool? SamlExempt { get; set; }

	/// <summary>
	/// The number of hours per week this person is available to work, in seconds; on a new user Harvest's default is 126000
	/// (35 hours).
	/// </summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>
	/// The billable rate to use for this user when they are added to a project; on a new user Harvest's default is 0.
	/// </summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>
	/// The cost rate to use for this user when calculating a project's costs vs billable amount; on a new user Harvest's
	/// default is 0.
	/// </summary>
	public decimal? CostRate { get; set; }

	/// <summary>Descriptive names of the business roles assigned to this person.</summary>
	public List<string>? Roles { get; set; }

	/// <summary>
	/// Access role(s) that determine the user's permissions in Harvest: <c>administrator</c>, <c>manager</c> or
	/// <c>member</c>, plus manager permissions such as <c>project_creator</c> or <c>expenses_manager</c>. On a new user
	/// Harvest's default is <c>member</c>.
	/// </summary>
	public List<string>? AccessRoles { get; set; }

	/// <summary>The name of the user's permissions profile.</summary>
	public string? PermissionsProfile { get; set; }
}