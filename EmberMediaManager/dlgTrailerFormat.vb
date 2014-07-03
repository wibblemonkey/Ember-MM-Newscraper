﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports EmberAPI
Imports System.Text.RegularExpressions

Public Class dlgTrailerFormat

#Region "Fields"

    Private WithEvents YouTube As YouTube.Scraper
    Private WithEvents IMDb As IMDb.Scraper
    Private _selectedformaturl As String
    Private _url As String
    Private _isYouTube As Boolean
    Private _isIMDb As Boolean

#End Region 'Fields

#Region "Methods"

    Public Overloads Function ShowDialog(ByVal URL As String) As String
        Me._url = URL

        If MyBase.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Return _selectedformaturl
        Else
            Return String.Empty
        End If
    End Function

    Private Sub dlgTrailerFormat_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        prbStatus.Style = ProgressBarStyle.Marquee
        Application.DoEvents()

        If Regex.IsMatch(Me._url, "https?:\/\/.*youtube.*\/watch\?v=(.{11})&?.*") Then
            _isIMDb = False
            _isYouTube = True
            YouTube.GetVideoLinks(Me._url)
            If YouTube.VideoLinks.Count > 0 Then
                Me.pnlStatus.Visible = False

                lbFormats.DataSource = YouTube.VideoLinks.Values.ToList
                lbFormats.DisplayMember = "Description"
                lbFormats.ValueMember = "URL"

                If YouTube.VideoLinks.ContainsKey(Master.eSettings.MovieTrailerPrefQual) Then
                    Me.lbFormats.SelectedIndex = YouTube.VideoLinks.IndexOfKey(Master.eSettings.MovieTrailerPrefQual)
                ElseIf Me.lbFormats.Items.Count = 1 Then
                    Me.lbFormats.SelectedIndex = 0
                End If
                Me.lbFormats.Enabled = True
            End If

        ElseIf Regex.IsMatch(Me._url, "http:\/\/.*imdb.*") Then
            _isIMDb = True
            _isYouTube = False
            IMDb.GetVideoLinks(Me._url)
            If IMDb.VideoLinks.Count > 0 Then
                Me.pnlStatus.Visible = False

                lbFormats.DataSource = IMDb.VideoLinks.Values.ToList
                lbFormats.DisplayMember = "Description"
                lbFormats.ValueMember = "URL"

                If IMDb.VideoLinks.ContainsKey(Master.eSettings.MovieTrailerPrefQual) Then
                    Me.lbFormats.SelectedIndex = IMDb.VideoLinks.IndexOfKey(Master.eSettings.MovieTrailerPrefQual)
                ElseIf Me.lbFormats.Items.Count = 1 Then
                    Me.lbFormats.SelectedIndex = 0
                End If
                Me.lbFormats.Enabled = True
            End If
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dlgTrailerFormat_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.SetUp()

            lbFormats.DataSource = Nothing

            YouTube = New YouTube.Scraper
            IMDb = New IMDb.Scraper

        Catch ex As Exception
            MsgBox(Master.eLang.GetString(921, "The video format links could not be retrieved."), MsgBoxStyle.Critical, Master.eLang.GetString(72, "Error Retrieving Video Format Links"))
        End Try
    End Sub

    Private Sub lstFormats_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbFormats.SelectedIndexChanged
        Try
            If _isYouTube Then
                Me._selectedformaturl = DirectCast(lbFormats.SelectedItem, YouTube.VideoLinkItem).URL
            ElseIf _isIMDb Then
                Me._selectedformaturl = DirectCast(lbFormats.SelectedItem, IMDb.VideoLinkItem).URL
            End If

            If Me.lbFormats.SelectedItems.Count > 0 Then
                Me.OK_Button.Enabled = True
            Else
                Me.OK_Button.Enabled = False
            End If
        Catch
        End Try
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub SetUp()
        Me.Text = Master.eLang.GetString(923, "Select Format")
        Me.lblStatus.Text = Master.eLang.GetString(924, "Getting available formats...")
        Me.gbFormats.Text = Master.eLang.GetString(925, "Available Formats")
        Me.OK_Button.Text = Master.eLang.GetString(179, "OK")
        Me.Cancel_Button.Text = Master.eLang.GetString(167, "Cancel")
    End Sub


#End Region 'Methods

End Class