﻿//using Hawaso.Models.Notes;
//using Microsoft.AspNetCore.Razor.TagHelpers;

//namespace DotNetNote.TagHelpers;

//[HtmlTargetElement("dnn-main-summary")]
//public class DotNetNoteMainSummaryTagHelper(INoteRepository repository) : TagHelper
//{
//    /// <summary>
//    /// 게시판 카테고리: Notice, Free, Data, Qna, ...
//    /// </summary>
//    public string Category { get; set; }

//    public override void Process(
//        TagHelperContext context, TagHelperOutput output)
//    {
//        output.TagName = "div";            

//        string s = "";

//        //var list = _repository.GetNoteSummaryByCategory(Category);
//        var list = repository.GetNoteSummaryByCategoryCache(Category);

//        foreach (var l in list)
//        {
//            s += $"<div class='post_item'><div class='post_item_text'>" 
//                + $"<span class='post_date'>" 
//                + l.PostDate.ToString("yyyy-MM-dd") 
//                + "</span><span class='post_title'>" 
//                + "<a href = '/DotNetNote/Details/" + l.Id + "'>" 
//                + Dul.StringLibrary.CutStringUnicode(l.Title, 33) 
//                + "</a></span></div></div>";
//        }

//        output.Content.AppendHtml(s);
//    }
//}
