using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace AWSLambdaFileWatermark
{
    class ConvertPDF
    {

        //private static string fontPath = Path.Combine(Directory.GetCurrentDirectory(), @"font\SIMFANG.TTF");
        private static string fontPath = "https://s3.cn-northwest-1.amazonaws.com.cn/hx-fxzl/font/SIMFANG.TTF";
        /// <summary>
        /// 创建一个显示指定图片的pdf
        /// </summary>
        /// <param name="picPdfPath"></param>
        /// <param name="picPath"></param>
        /// <returns></returns>
        public static bool CreatePDFByPic(string picPdfPath, string picPath)
        {
            //新建一个文档
            Document doc = new Document();
            try
            {
                //建立一个书写器(Writer)与document对象关联
                PdfWriter.GetInstance(doc, new FileStream(picPdfPath, FileMode.Create, FileAccess.ReadWrite));
                //打开一个文档
                doc.Open();
                //向文档中添加内容
                Image img = Image.GetInstance(picPath);
                //img.SetAbsolutePosition();
                doc.Add(img);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();
                }
            }

        }

        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="ModelPicName"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static bool PDFWatermark(string inputfilepath, string outputfilepath, string ModelPicName, float top, float left)
        {
            //throw new NotImplementedException();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);

                int numberOfPages = pdfReader.NumberOfPages;

                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);

                float width = psize.Width;

                float height = psize.Height;

                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));

                PdfContentByte waterMarkContent;

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(ModelPicName);

                image.GrayFill = 20;//透明度，灰色填充
                //image.Rotation//旋转
                //image.RotationDegrees//旋转角度
                //水印的位置
                if (left < 0)
                {
                    left = width / 2 - image.Width + left;
                }

                //image.SetAbsolutePosition(left, (height - image.Height) - top);
                image.SetAbsolutePosition(left, (height / 2 - image.Height) - top);


                //每一页加水印,也可以设置某一页加水印
                for (int i = 1; i <= numberOfPages; i++)
                {
                    //waterMarkContent = pdfStamper.GetUnderContent(i);//内容下层加水印
                    waterMarkContent = pdfStamper.GetOverContent(i);//内容上层加水印

                    waterMarkContent.AddImage(image);
                }
                //strMsg = "success";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        /// <summary>
        /// 添加倾斜水印
        /// </summary>
        /// <param name="pdfStream">pdf文件流</param>
        /// <param name="waterMarkName">水印字符串</param>
        public static Stream SetWatermark(Stream pdfStream, string waterMarkName)
        {
            try
            {
                int fontSize = 20;//设置字体大小
                int span = 40;//设置垂直位移
                Stream outStream = new MemoryStream();
                PdfReader pdfReader = new PdfReader(pdfStream);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, outStream);
                Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                pdfStamper.Writer.CloseStream = false;
                int total = pdfReader.NumberOfPages + 1;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);//华文云彩字体
                PdfGState gs = new PdfGState
                {
                    FillOpacity = 0.15f//透明度
                };
                int waterMarkNameLenth = waterMarkName.Length;
                char c;
                int rise = 0;
                string spanString = " ";//水平位移
                for (int i = 1; i < total; i++)
                {
                    rise = waterMarkNameLenth * span;
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    content.SetGState(gs);
                    content.BeginText();
                    content.SetColorFill(BaseColor.Red);
                    content.SetFontAndSize(font, fontSize);
                    int heightNumbert = (int)Math.Ceiling((decimal)height / (decimal)rise);//垂直重复的次数，进一发
                    int panleWith = (fontSize + span) * waterMarkNameLenth;
                    int widthNumber = (int)Math.Ceiling((decimal)width / (decimal)panleWith);//水平重复次数

                    // 设置水印文字字体倾斜 开始 
                    for (int w = 0; w < widthNumber; w++)
                    {
                        for (int h = 1; h <= heightNumbert; h++)
                        {
                            int yleng = rise * h;
                            content.SetTextMatrix(w * panleWith, yleng);//x,y设置水印开始的绝对左边，以左下角为x，y轴的起点
                            for (int k = 0; k < waterMarkNameLenth; k++)
                            {
                                content.SetTextRise(yleng);//指定的y轴值处添加
                                c = waterMarkName[k];
                                content.ShowText(c + spanString);
                                yleng -= span;
                            }
                        }
                    }
                    content.EndText();
                }
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();

                return outStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 添加普通偏转角度文字水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="permission"></param>
        public static void SetWatermark(string inputfilepath, string outputfilepath, string waterMarkName)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                int total = pdfReader.NumberOfPages + 1;
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                for (int i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    //content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    //透明度
                    gs.FillOpacity = 0.3f;
                    content.SetGState(gs);
                    //content.SetGrayFill(0.3f);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(BaseColor.LightGray);
                    content.SetFontAndSize(font, 100);
                    content.SetTextMatrix(0, 0);
                    content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, width / 2 - 50, height / 2 - 50, 55);
                    //content.SetColorFill(BaseColor.BLACK);
                    //content.SetFontAndSize(font, 8);
                    //content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, 0, 0, 0);
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        /// <summary>
        /// 添加普通偏转角度文字水印
        /// </summary>
        /// <param name="isp">文件流</param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="permission"></param>
        public static void SetWatermark(Stream isp, string outputfilepath, string waterMarkName)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(isp);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                int total = pdfReader.NumberOfPages + 1;
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                for (int i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    //透明度
                    gs.FillOpacity = 0.3f;
                    content.SetGState(gs);
                    content.SetGrayFill(0.3f);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(BaseColor.LightGray);
                    content.SetFontAndSize(font, 100);
                    content.SetTextMatrix(0, 0);
                    content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, width / 2 - 50, height / 2 - 50, 55);
                    //content.SetColorFill(BaseColor.BLACK);
                    //content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, 0, 0, 0);
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        /// <summary>
        /// 添加倾斜水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="userPassWord"></param>
        /// <param name="ownerPassWord"></param>
        /// <param name="permission"></param>
        public static void SetWatermark(string inputfilepath, string outputfilepath, string waterMarkName, string userPassWord, string ownerPassWord, int permission)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                // 设置密码  
                //pdfStamper.SetEncryption(false,userPassWord, ownerPassWord, permission);

                int total = pdfReader.NumberOfPages + 1;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                gs.FillOpacity = 0.2f;//透明度

                int j = waterMarkName.Length;
                char c;
                int rise = 0;
                for (int i = 1; i < total; i++)
                {
                    rise = 500;
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                                                           //content = pdfStamper.GetUnderContent(i);//在内容下方加水印

                    content.BeginText();
                    content.SetColorFill(BaseColor.DarkGray);
                    content.SetFontAndSize(font, 50);
                    // 设置水印文字字体倾斜 开始
                    if (j >= 15)
                    {
                        content.SetTextMatrix(200, 120);
                        for (int k = 0; k < j; k++)
                        {
                            content.SetTextRise(rise);
                            c = waterMarkName[k];
                            content.ShowText(c + "");
                            rise -= 20;
                        }
                    }
                    else
                    {
                        content.SetTextMatrix(180, 100);
                        for (int k = 0; k < j; k++)
                        {
                            content.SetTextRise(rise);
                            c = waterMarkName[k];
                            content.ShowText(c + "");
                            rise -= 18;
                        }
                    }
                    // 字体设置结束
                    content.EndText();
                    // 画一个圆
                    //content.Ellipse(250, 450, 350, 550);
                    //content.SetLineWidth(1f);
                    //content.Stroke();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

    }
}
