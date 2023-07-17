using System;
using System.Diagnostics;
using Android.Graphics;
using Android.Text;
using Master.Flame.Danmaku.Danmaku.Model;
using Master.Flame.Danmaku.Danmaku.Model.Android;
using Master.Flame.Danmaku.Danmaku.Parser;
using Master.Flame.Danmaku.Danmaku.Parser.Android;
using Master.Flame.Danmaku.Danmaku.Util;
using Org.Json;
using Org.Xml.Sax;
using Org.Xml.Sax.Helpers;
using Danmakus = Master.Flame.Danmaku.Danmaku.Model.Android.Danmakus;

namespace Master.Flame.Danmaku.Additions
{
    public sealed class BiliDanmakuParser : BaseDanmakuParser
    {
        public float DispScaleX;
        public float DispScaleY;
        static BiliDanmakuParser()
        {
            Java.Lang.JavaSystem.SetProperty("org.xml.sax.driver", "org.xmlpull.v1.sax2.Driver");
        }
        protected override IDanmakus Parse()
        {
            try
            {
                if (MDataSource != null)
                {
                    AndroidFileSource source = (AndroidFileSource)MDataSource;
                    IXMLReader xmlReader = XMLReaderFactory.CreateXMLReader();
                    XmlContentHandler contentHandler = new XmlContentHandler(this);
                    xmlReader.ContentHandler = contentHandler;
                    var inputSource = new InputSource(source.DataStream);
                    xmlReader.Parse(inputSource);
                    return contentHandler.Result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return new Danmakus();
        }
        public override BaseDanmakuParser SetDisplayer(IDisplayer disp)
        {
            base.SetDisplayer(disp);
            DispScaleX = MDispWidth / DanmakuFactory.BiliPlayerWidth;
            DispScaleY = MDispHeight / DanmakuFactory.BiliPlayerHeight;
            return this;
        }
        public class XmlContentHandler : DefaultHandler
        {
            private const string TRUE_STRING = "true";
            public Danmakus Result { get; set; }
            public BaseDanmaku item = null;
            public bool completed = false;
            public int index = 0;
            public BiliDanmakuParser DanmakuParser { get; }
            public XmlContentHandler(BiliDanmakuParser parser)
            {
                DanmakuParser = parser;
            }
            public override void StartDocument()
            {
                Result = new Danmakus(0, false, DanmakuParser.MContext.BaseComparator);
            }
            public override void EndDocument()
            {
                completed = true;
            }
            public override void StartElement(string uri, string localName, string qName, IAttributes attributes)
            {
                string tagName = localName.Length != 0 ? localName : qName;
                tagName = tagName.ToLower().Trim();
                if (tagName.Equals("d"))
                {
                    string pValue = attributes.GetValue("p");
                    string[] values = pValue.Split(",");
                    if (values.Length > 0)
                    {
                        long time = (long)(values[0].ToSingle() * 1000);
                        int type = values[1].ToInt32();
                        float textSize = values[2].ToSingle();
                        int color = (int)((0x00000000ff000000 | values[3].ToInt64()) & 0x00000000ffffffff);
                        item = DanmakuParser.MContext.MDanmakuFactory.CreateDanmaku(type, DanmakuParser.MContext);
                        if (item != null)
                        {
                            item.Time = time;
                            item.TextSize = textSize * (DanmakuParser.MDispDensity - 0.6f);
                            item.TextColor = color;
                            item.TextShadowColor = color <= Color.Black ? Color.White : Color.Black;
                        }
                    }
                }
            }
            public override void EndElement(string uri, string localName, string qName)
            {
                if (item != null && item.Text != null)
                {
                    string tagName = localName.Length != 0 ? localName : qName;
                    if (tagName.Equals("d", StringComparison.OrdinalIgnoreCase))
                    {
                        item.Timer = DanmakuParser.MTimer;
                        item.Flags = DanmakuParser.MContext.MGlobalFlagValues;
                        var lockObj = Result.ObtainSynchronizer();
                        lock(lockObj)
                            Result.AddItem(item);
                    }
                }
                item = null;
            }
            public override void Characters(char[] ch, int start, int length)
            {
                if (item != null)
                {
                    DanmakuUtils.FillText(item, DecodeXmlString(new string(ch, start, length)));
                    item.Index = index++;
                    string text = item.Text.ToString().Trim();
                    if (item.Type == BaseDanmaku.TypeSpecial && text.StartsWith("[") && text.EndsWith("]"))
                    {
                        string[] textArr = null;
                        try
                        {
                            JSONArray jsonArray = new JSONArray(text);
                            textArr = new string[jsonArray.Length()];
                            for (int i = 0; i < textArr.Length; i++)
                            {
                                textArr[i] = jsonArray.GetString(i);
                            }
                        }
                        catch { }
                        if (textArr == null || textArr.Length < 5 || TextUtils.IsEmpty(textArr[4]))
                        {
                            item = null;
                            return;
                        }
                        DanmakuUtils.FillText(item, textArr[4]);
                        float beginX = textArr[0].ToUInt32();
                        float beginY = textArr[1].ToUInt32();
                        float endX = beginX;
                        float endY = beginY;
                        string[] alphaArr = textArr[2].Split("-");
                        int beginAlpha = (int)(AlphaValue.Max * alphaArr[0].ToUInt32());
                        int endAlpha = beginAlpha;
                        if (alphaArr.Length > 1)
                            endAlpha = (int)(AlphaValue.Max * alphaArr[1].ToUInt32());
                        long alphaDuraion = textArr[3].ToUInt32() * 1000;
                        long translationDuration = alphaDuraion;
                        long translationStartDelay = 0;
                        float rotateY = 0, rotateZ = 0;
                        if (textArr.Length >= 7)
                        {
                            rotateZ = textArr[5].ToUInt32();
                            rotateY = textArr[6].ToUInt32();
                        }
                        if (textArr.Length >= 11)
                        {
                            endX = textArr[7].ToUInt32();
                            endY = textArr[8].ToUInt32();
                            if (!"".Equals(textArr[9]))
                                translationDuration = textArr[9].ToInt32();
                            if (!"".Equals(textArr[10]))
                                translationStartDelay = textArr[10].ToUInt32();
                        }
                        if (IsPercentageNumber(textArr[0]))
                            beginX *= DanmakuFactory.BiliPlayerWidth;
                        if (IsPercentageNumber(textArr[1]))
                            beginY *= DanmakuFactory.BiliPlayerHeight;
                        if (textArr.Length >= 8 && IsPercentageNumber(textArr[7]))
                            endX *= DanmakuFactory.BiliPlayerWidth;
                        if (textArr.Length >= 9 && IsPercentageNumber(textArr[8]))
                            endY *= DanmakuFactory.BiliPlayerHeight;
                        item.SetDuration(new Duration(alphaDuraion));
                        item.RotationZ = rotateZ;
                        item.RotationY = rotateY;
                        DanmakuParser.MContext.MDanmakuFactory.FillTranslationData(item, beginX,
                                beginY, endX, endY, translationDuration, translationStartDelay, DanmakuParser.DispScaleX, DanmakuParser.DispScaleY);
                        DanmakuParser.MContext.MDanmakuFactory.FillAlphaData(item, beginAlpha, endAlpha, alphaDuraion);
                        if (textArr.Length >= 12)
                        {
                            if (!TextUtils.IsEmpty(textArr[11]) && TRUE_STRING.Equals(textArr[11], StringComparison.OrdinalIgnoreCase))
                                item.TextShadowColor = Color.Transparent;
                        }
                        if (textArr.Length >= 14)
                            ((SpecialDanmaku)item).IsQuadraticEaseOut = ("0".Equals(textArr[13]));
                        if (textArr.Length >= 15)
                        {
                            if (!"".Equals(textArr[14]))
                            {
                                string motionPathString = textArr[14].Substring(1);
                                if (!TextUtils.IsEmpty(motionPathString))
                                {
                                    string[] pointStrArray = motionPathString.Split("L");
                                    if (pointStrArray.Length > 0)
                                    {
                                        float[][] points = new float[pointStrArray.Length][];
                                        for (int i = 0; i < pointStrArray.Length; i++)
                                        {
                                            string[] pointArray = pointStrArray[i].Split(",");
                                            if (pointArray.Length >= 2)
                                            {
                                                points[i][0] = pointArray[0].ToUInt32();
                                                points[i][1] = pointArray[1].ToUInt32();
                                            }
                                        }
                                        DanmakuParser.MContext.MDanmakuFactory.CustomFillLinePathData(item, points, DanmakuParser.DispScaleX, DanmakuParser.DispScaleY);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            private bool IsPercentageNumber(string number)
            {
                return number != null && number.Contains(".");
            }
            private string DecodeXmlString(string title)
            {
                if (title.Contains("&amp;"))
                    title = title.Replace("&amp;", "&");
                if (title.Contains("&quot;"))
                    title = title.Replace("&quot;", "\"");
                if (title.Contains("&gt;"))
                    title = title.Replace("&gt;", ">");
                if (title.Contains("&lt;"))
                    title = title.Replace("&lt;", "<");
                return title;
            }
        }
    }
}