Imports System.Data.SqlClient
Imports System.IdentityModel.Tokens.Jwt
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Security.Cryptography
Imports System.Web.Http
Imports Microsoft.IdentityModel.Tokens
Imports Newtonsoft.Json

Public Class ValuesController
    Inherits ApiController

    ' GET api/values
    <HttpPost>
    <Route("api/Webhuk_rawdata_qiscus")>
    Public Function Webhuk_rawdata_qiscus() As HttpResponseMessage
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Dim notification As String = GetRequestBody()

            Using connection As New SqlConnection(connectionString)
                Using command As New SqlCommand("webhuk_tr", connection)
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.AddWithValue("@GetTR_Response", notification)

                    connection.Open()
                    command.ExecuteNonQuery()
                End Using
            End Using

            Return CreateSuccessResponse()
        Catch ex As Exception
            Return CreateErrorResponse(ex)
        End Try
    End Function

    Private Function GetRequestBody() As String
        Using stream As New MemoryStream()
            Dim request = HttpContext.Current.Request
            request.InputStream.Seek(0, SeekOrigin.Begin)
            request.InputStream.CopyTo(stream)

            ' Log request body
            WriteToLog("Request Body: " & Encoding.UTF8.GetString(stream.ToArray()))

            ' Membuat response sukses
            CreateSuccessResponse()

            ' Kembalikan request body sebagai string
            Return Encoding.UTF8.GetString(stream.ToArray())
        End Using
    End Function

    ' Fungsi untuk membuat respons sukses dan mencatat log
    Private Function CreateSuccessResponse() As HttpResponseMessage
        Dim response = New HttpResponseMessage(HttpStatusCode.OK) With {
            .Content = New StringContent("success", Encoding.UTF8, "application/json")
        }

        ' Log respons sukses
        WriteToLog("Response Status: OK, Content: success")

        Return response
    End Function

    ' Fungsi untuk menulis log ke file
    Private Sub WriteToLog(message As String)
        Dim logDirectory As String = "C:\log" ' Path folder log
        Dim logFilePath As String = Path.Combine(logDirectory, "logfile.txt") ' Path file log

        Try
            ' Pastikan folder log ada
            If Not Directory.Exists(logDirectory) Then
                Directory.CreateDirectory(logDirectory)
            End If

            ' Menulis log ke file
            Using writer As New StreamWriter(logFilePath, True)
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}")
            End Using
        Catch ex As Exception
            ' Tangani error jika gagal menulis ke file log
            Console.WriteLine("Error writing to log: " & ex.Message)
        End Try
    End Sub

    Private Function CreateErrorResponse(ex As Exception) As HttpResponseMessage
        Dim jsonObject = New With {
        .status = 401,
        .message = "error: " + ex.ToString()
    }
        Dim response = New HttpResponseMessage(HttpStatusCode.InternalServerError) With {
        .Content = New StringContent(JsonConvert.SerializeObject(jsonObject, Formatting.Indented), Encoding.UTF8, "application/json")
    }
        WriteToLog("Response Status: error, Content:" + ex.ToString())

        Return response
    End Function

    




    Public Class ValueItem
        Public Property Name As String
        Public Property Alamat As String
        Public Property Email As String
        Public Property NoHp As String

    End Class
    Public Class UserModel
        Public Property userName As String
        Public Property Password As String


    End Class
    Public Class ValueChannelModel
        Public Property ValueChannel As String



    End Class

    ' GET api/values/5
    Public Function GetValue(ByVal id As Integer) As String
        Return "value"
    End Function

    ' POST api/values
    Public Sub PostValue(<FromBody()> ByVal value As String)

    End Sub

    ' PUT api/values/5
    Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

    End Sub

    ' DELETE api/values/5
    Public Sub DeleteValue(ByVal id As Integer)

    End Sub

End Class
