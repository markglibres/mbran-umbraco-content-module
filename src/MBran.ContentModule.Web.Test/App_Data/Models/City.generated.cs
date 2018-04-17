//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	/// <summary>City</summary>
	[PublishedContentModel("city")]
	public partial class City : PublishedContentModel, IHasTextAndMedia
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "city";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public City(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<City, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Image
		///</summary>
		[ImplementPropertyType("textMediaImage")]
		public IPublishedContent TextMediaImage
		{
			get { return Umbraco.Web.PublishedContentModels.HasTextAndMedia.GetTextMediaImage(this); }
		}

		///<summary>
		/// Summary
		///</summary>
		[ImplementPropertyType("textMediaSummary")]
		public string TextMediaSummary
		{
			get { return Umbraco.Web.PublishedContentModels.HasTextAndMedia.GetTextMediaSummary(this); }
		}

		///<summary>
		/// Title
		///</summary>
		[ImplementPropertyType("textMediaTitle")]
		public string TextMediaTitle
		{
			get { return Umbraco.Web.PublishedContentModels.HasTextAndMedia.GetTextMediaTitle(this); }
		}
	}
}