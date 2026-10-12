namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/contacts</c>.</summary>
public class ContactsContainer : ListContainerBase
{
	/// <summary>The contacts on this page (<c>contacts</c>).</summary>
	public List<Contact> Contacts { get; set; } = [];
}