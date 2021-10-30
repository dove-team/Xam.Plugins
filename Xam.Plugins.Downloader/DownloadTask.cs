﻿using Android.OS;
using Java.IO;
using Square.OkHttp3;
using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using File = Java.IO.File;
using Debug = System.Diagnostics.Debug;
using Environment = Android.OS.Environment;

namespace Xam.Plugins.Downloader
{
    public sealed class DownloadTask : AsyncTask<string, int, int>
    {
        public const int TYPE_SUCCESS = 0;
        public const int TYPE_FAILED = 1;
        public const int TYPE_PAUSED = 2;
        public const int TYPE_CANCELED = 3;
        private int lastProgress;
        private bool isCanceled = false;
        private bool isPaused = false;
        private IDownloadListener Listener { get; }
        public DownloadTask(IDownloadListener listener)
        {
            Listener = listener;
        }
        protected override int RunInBackground(params string[] parms)
        {
            Stream inputStream = null;
            RandomAccessFile savedFile = null;
            File file = null;
            try
            {
                long downloadedLength = 0;
                string downloadUrl = parms[0];
                string fileName = downloadUrl[downloadUrl.LastIndexOf("/")..];
                string directory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).Path;
                file = new File(directory + fileName);
                if (file.Exists())
                    downloadedLength = file.Length();
                long contentLength = GetContentLength(downloadUrl);
                if (contentLength == 0)
                    return TYPE_FAILED;
                else if (contentLength == downloadedLength)
                    return TYPE_SUCCESS;
                OkHttpClient client = new OkHttpClient();
                Request request = new Request.Builder().AddHeader("RANGE", "bytes=" + downloadedLength + "-").Url(downloadUrl).Build();
                Response response = client.NewCall(request).Execute();
                if (response != null)
                {
                    inputStream = response.Body().ByteStream();
                    savedFile = new RandomAccessFile(file, "rw");
                    savedFile.Seek(downloadedLength);
                    byte[] b = new byte[1024];
                    int len, total = 0;
                    while ((len = inputStream.Read(b)) != -1)
                    {
                        if (isCanceled)
                            return TYPE_CANCELED;
                        else if (isPaused)
                            return TYPE_PAUSED;
                        else
                        {
                            total += len;
                            savedFile.Write(b, 0, len);
                            int progress = (int)((total + downloadedLength) * 100 / contentLength);
                            PublishProgress(progress);
                        }
                    }
                    response.Body().Close();
                    return TYPE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                try
                {
                    inputStream?.Close();
                    savedFile?.Close();
                    if (isCanceled && file != null)
                        file.Delete();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return TYPE_FAILED;
        }
        protected override void OnProgressUpdate(params int[] values)
        {
            int progress = values[0];
            if (progress > lastProgress)
            {
                Listener.OnProgress(progress);
                lastProgress = progress;
            }
        }
        protected override void OnPostExecute([AllowNull] int result)
        {
            switch (result)
            {
                case TYPE_SUCCESS:
                    Listener.OnSuccess();
                    break;
                case TYPE_FAILED:
                    Listener.OnFailed();
                    break;
                case TYPE_PAUSED:
                    Listener.OnPaused();
                    break;
                case TYPE_CANCELED:
                    Listener.OnCanceled();
                    break;
                default:
                    break;
            }
        }
        public void PauseDownload() => isPaused = true;
        public void CancelDownload() => isCanceled = true;
        private long GetContentLength(string downloadUrl)
        {
            OkHttpClient client = new OkHttpClient();
            Request request = new Request.Builder().Url(downloadUrl).Build();
            Response response = client.NewCall(request).Execute();
            if (response != null && response.IsSuccessful)
            {
                long contentLength = response.Body().ContentLength();
                response.Body().Close();
                return contentLength;
            }
            return 0;
        }
    }
}