using System;
using System.Linq;
using Dataflip;

namespace Hello.World
{
	public class CoolContext : DataflipContext
	{
		public CoolContext(DataflipSettings settings)
		: 
		base(settings)
		{}

		public CoolContext()
		: 
		base("CoolContext")
		{}

		#region Result Classes
		public class GetActivitiesResult
		{
			public Int32? ActivityId { get; set; } 
			public String Content { get; set; } 
			public String ActivityType { get; set; } 
			public String ActivityImage { get; set; } 
			public DateTime? CreatedDate { get; set; } 
			public String Name { get; set; } 
			public String SmallImageUrl { get; set; } 
			public String LargeImageUrl { get; set; } 
		}

		#endregion


		#region GetActivities
		public class GetActivitiesParameters
		{
			public String Name { get; set; } 
			public String Description { get; set; } 
			public Int32? SomeInt { get; set; } 
		}
		///<summary>
		///This gets all the activities
		///</summary>
		public IEnumerable<GetActivitiesResult> GetActivities(GetActivitiesParameters parameters)
		{
			return new SqlQuery(Settings).ExecuteObjectArray(
				query : "GetActivities",
				parameters : _ =>
				{
					_.AddWithValue("@Name", parameters.Name);
					_.AddWithValue("@Description", parameters.Description);
					_.AddWithValue("@SomeInt", parameters.SomeInt);
				},
				mapping: reader => new GetActivitiesResult()
				{
					ActivityId = reader["ActivityId"] as Int32?,
					Content = reader["Content"] as String,
					ActivityType = reader["ActivityType"] as String,
					ActivityImage = reader["ActivityImage"] as String,
					CreatedDate = reader["CreatedDate"] as DateTime?,
					Name = reader["Name"] as String,
					SmallImageUrl = reader["SmallImageUrl"] as String,
					LargeImageUrl = reader["LargeImageUrl"] as String
				}
			);
		}
		#endregion

		#region ap_ActionHistory_Retrieve
		public class RetrieveActionHistoryParameters
		{
			public Int32? ActionHistoryId { get; set; } 
			public Int32? AcctionId { get; set; } 
			public DateTime? AcctionTime { get; set; } 
			public Int32? SesionId { get; set; } 
		}
		public int RetrieveActionHistory(RetrieveActionHistoryParameters parameters)
		{
			return new SqlQuery(Settings).ExecuteNonQuery(
				query : "ap_ActionHistory_Retrieve",
				parameters : _ =>
				{
					_.AddWithValue("@ActionHistoryId", parameters.ActionHistoryId);
					_.AddWithValue("@AcctionId", parameters.AcctionId);
					_.AddWithValue("@AcctionTime", parameters.AcctionTime);
					_.AddWithValue("@SesionId", parameters.SesionId);
				}
			);
		}
		#endregion

		#region ap_ActionHistory_Retrieve
		public class RetrieveActionHistory2Parameters
		{
			public Int32? ActionHistoryId { get; set; } 
			public Int32? AcctionId { get; set; } 
			public DateTime? AcctionTime { get; set; } 
			public Int32? SesionId { get; set; } 
		}
		public Int32 RetrieveActionHistory2(RetrieveActionHistory2Parameters parameters)
		{
			return (Int32) new SqlQuery(Settings).ExecuteScalar(
				query : "ap_ActionHistory_Retrieve",
				parameters : _ =>
				{
					_.AddWithValue("@ActionHistoryId", parameters.ActionHistoryId);
					_.AddWithValue("@AcctionId", parameters.AcctionId);
					_.AddWithValue("@AcctionTime", parameters.AcctionTime);
					_.AddWithValue("@SesionId", parameters.SesionId);
				}
			);
		}
		#endregion

	}
}
