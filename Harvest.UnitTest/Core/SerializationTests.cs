using Harvest.Serialization;
using System.Text.Json;

namespace Harvest.UnitTest.Core;

public class SerializationTests
{
	[Theory]
	[InlineData("8:00am", 8, 0)]
	[InlineData("12:30pm", 12, 30)]
	[InlineData("12:05am", 0, 5)]
	[InlineData("1:00PM", 13, 0)]
	[InlineData("08:15am", 8, 15)]
	[InlineData("9:45 pm", 21, 45)]
	[InlineData("13:00", 13, 0)]
	[InlineData("7:05", 7, 5)]
	[InlineData(" 23:59 ", 23, 59)]
	public void HarvestTime_ParsesBothClocks(string value, int hours, int minutes)
		=> HarvestTime.Parse(value).Should().Be(new TimeSpan(hours, minutes, 0));

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("noon")]
	[InlineData("25:00")]
	[InlineData("8:00xm")]
	public void HarvestTime_UnrecognisedValue_IsNull(string? value)
		=> HarvestTime.Parse(value).Should().BeNull();

	private sealed class Holder
	{
		[System.Text.Json.Serialization.JsonConverter(typeof(FlexibleStringConverter))]
		public string? Value { get; set; }
	}

	[Theory]
	[InlineData("""{"Value":"abc"}""", "abc")]
	[InlineData("""{"Value":123}""", "123")]
	[InlineData("""{"Value":-1.5e3}""", "-1.5e3")]
	[InlineData("""{"Value":true}""", "true")]
	[InlineData("""{"Value":false}""", "false")]
	[InlineData("""{"Value":null}""", null)]
	public void FlexibleStringConverter_ReadsStringsNumbersAndBooleans(string json, string? expected)
		=> JsonSerializer.Deserialize<Holder>(json)!.Value.Should().Be(expected);

	[Fact]
	public void FlexibleStringConverter_RejectsObjects()
	{
		var act = () => JsonSerializer.Deserialize<Holder>("""{"Value":{}}""");

		act.Should().Throw<JsonException>();
	}

	[Fact]
	public void FlexibleStringConverter_WritesAString()
		=> JsonSerializer.Serialize(new Holder { Value = "12" }).Should().Be("""{"Value":"12"}""");

	[Fact]
	public void UrlParameterFormatter_FormatsHarvestValues()
	{
		var formatter = new HarvestUrlParameterFormatter();
		var provider = typeof(SerializationTests);

		formatter.Format(true, provider, typeof(bool)).Should().Be("true");
		formatter.Format(false, provider, typeof(bool)).Should().Be("false");
		formatter.Format(new DateTimeOffset(2017, 6, 26, 23, 52, 18, TimeSpan.FromHours(2)), provider, typeof(DateTimeOffset))
			.Should().Be("2017-06-26T21:52:18Z");
		formatter.Format(new DateOnly(2017, 6, 26), provider, typeof(DateOnly)).Should().Be("2017-06-26");
		formatter.Format(42L, provider, typeof(long)).Should().Be("42");
		formatter.Format(null, provider, typeof(string)).Should().BeNull();
	}

	[Fact]
	public void PaginationLinks_NextCursor_ReadsTheCursorFromTheNextLink()
	{
		new PaginationLinks { Next = "https://api.harvestapp.com/v2/time_entries?cursor=eyJh%3D%3D&per_page=100" }
			.NextCursor.Should().Be("eyJh==");
		new PaginationLinks { Next = "https://api.harvestapp.com/v2/clients?page=2&per_page=100" }.NextCursor.Should().BeNull();
		new PaginationLinks { Next = null }.NextCursor.Should().BeNull();
		new PaginationLinks { Next = "not a url" }.NextCursor.Should().BeNull();
	}

	[Fact]
	public void UserAssignment_ReadsEveryField()
	{
		var assignment = JsonSerializer.Deserialize<UserAssignment>("""
			{"id":130403297,"is_project_manager":true,"is_active":true,"use_default_rates":false,"budget":null,
			"created_at":"2017-08-22T17:36:54Z","updated_at":"2017-08-22T17:36:54Z","hourly_rate":100.0,
			"project":{"id":14808188,"name":"Task Force","code":"TF"},"user":{"id":1782959,"name":"Kim Allen"}}
			""", HarvestClient.JsonOptions)!;

		assignment.Id.Should().Be(130403297);
		assignment.IsProjectManager.Should().BeTrue();
		assignment.IsActive.Should().BeTrue();
		assignment.UseDefaultRates.Should().BeFalse();
		assignment.Budget.Should().BeNull();
		assignment.HourlyRate.Should().Be(100m);
		assignment.Project!.Code.Should().Be("TF");
		assignment.User!.Name.Should().Be("Kim Allen");
		assignment.CreatedAt.Should().Be(new DateTime(2017, 8, 22, 17, 36, 54, DateTimeKind.Utc));
	}
}