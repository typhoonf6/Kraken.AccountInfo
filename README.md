## Build Status - In Progress

## Description
Firstly, I'm learning Xamarin and thought this a good way to do it. I had a very specific
use case that was annoying to me so decided to fix it by creating this app.

This  app that is solely designed to get public updates to Crypto assets from the Kraken API
and show changes to personal crypto assets without the need to sign in an provide OTP's every 
single time you open the generic app. It does this by just updating the tickers to calculate
the users holdings.

### Who is this for?
For those with long term investments that just want to be able to open the app once a day
and seehow their assets are doing without having to login and update private information. 
If you make consistent trades, this app won't help you as asset holdings are not updated 
unless you log in, which means you should likely just use the generic kraken app.

## Why?
### Problem
The generic Kraken app requires sign in to update personal details. This is annoying as, 
at least on my phone, if the app closes it forces me to have to sign in again and give a
new OTP.

### Solution
The user will login initially to pull their asset holdings, then any other refreshes will
will be from the public API to update the asset price only. The user will then be able
to initiate logins at their convenience to update asset holdings if necessary.

## Getting Started
Written in C# using Visual Studio 2019 and Xamarin.Forms
### Target
*Android
*iOS
*UWP
UWP only tested

### NuGets
* PropertyChanged.Fody 3.4.0
* NewtonSoft.Json 13.0.1
* Xamarin.CommunityToolkit 1.2.0
* SQLite-Net-PCL 1.7.335

### Possible Issues
I'm using QuickGraph and Djikstra to figure out the conversion rate for the asset to the chosen currency. Unfortunately this package isn't supported by Xamarin and bitches when compiling. I've tested UWP and it's working, but have yet to try android or iOS
