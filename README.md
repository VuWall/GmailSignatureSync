# GmailSignatureSync
Allows you to build a list of users and generate consistent, easily modifiable email signatures across your organization. 

*Example:*

![Sample Signature](http://i.imgur.com/zkYTAVi.png)

I looked high and low for applications that could synchronize Google Apps email signatures and not only is it not included in Gmail by default, but all the available solutions cost money and frankly don't work well or are terribly convoluted. Instead, I whipped up this small script to be able to quickly add new signatures, update existing signatures, and ensure consistent branding across the organization. Of course, you can change the HTML portion of the signature to fit your style / needs. 

What You'll Need:
-----------------
 - Google Apps admin account.
 - Google Developer Account associated with your domain.
 - Your @developer.gserviceaccount.com address.
 - PKCS 12 key linked to your Google Developer account.
