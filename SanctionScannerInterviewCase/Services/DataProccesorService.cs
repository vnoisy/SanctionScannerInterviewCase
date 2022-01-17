﻿using HtmlAgilityPack;
using SanctionScannerInterviewCase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanctionScannerInterviewCase.Services
{
    public class DataProccesorService
    {
        private string regexSpaces = "[\\r|\\n|\\t]\\s\\s+";
        private string regexHtml = @"<[^>]+>|&nbsp;";
        private string regexNumber = @"[^0-9.]";
        private string regexLetter = @"[^A-Z]";
        public List<MainPageModel> BuildMainPage(string data)
        {
            try
            {
                var model = new List<MainPageModel>();
                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                var findedHtml = doc.DocumentNode.SelectNodes(@"//*[@id='container']/div[3]/div/div[3]/div[3]/ul/li").Descendants("a");
                foreach (var item in findedHtml)
                {
                    model.Add(new MainPageModel
                    {
                        Url = item.Attributes["href"].Value,
                        Text = regexParser(item.InnerText, regexSpaces),
                    });
                }
                return model;
            }
            catch
            {
                return new List<MainPageModel>();
            }

        }

        public DetailPageModel BuildDetailPage(string data)
        {
            try
            {
                var attrList = new List<AttributeModel>();
                var detailPage = new DetailPageModel();
                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                var findedAttrHtml = doc.DocumentNode.SelectNodes(@"//*[@id='classifiedDetail']/div/div[2]/div[2]/ul/li");
                foreach (var item in findedAttrHtml)
                {
                    var title = item.Descendants("strong").FirstOrDefault();
                    var value = item.Descendants("span").FirstOrDefault();
                    attrList.Add(new AttributeModel
                    {
                        Title = title != null ? regexParser(title.InnerText, regexSpaces) : "",
                        Value = value != null ? regexParser(regexParser(value.InnerText, regexSpaces),regexHtml) : "",
                    });
                }
                var price = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()").InnerText, regexNumber);
                var currency = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()").InnerText, regexLetter);
                var detailTitle = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[1]/h1").InnerText, regexSpaces);
                var city = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h2/a[1]").InnerText, regexSpaces);
                var region = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h2/a[2]").InnerText, regexSpaces);
                var state = regexParser(doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h2/a[3]").InnerText, regexSpaces);
                var desc = doc.DocumentNode.SelectSingleNode("//*[@id='classifiedDescription']").InnerHtml;
                detailPage.AttributeList = attrList;
                detailPage.City = city;
                detailPage.Currency = currency;
                detailPage.Description = desc;
                detailPage.Price = decimal.Parse(price);
                detailPage.Region = region;
                detailPage.State = state;
                detailPage.Title = detailTitle;
                return detailPage;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string regexParser(string text,string regex)
        {
            return Regex.Replace(text, regex, string.Empty);
        }
    }
}
