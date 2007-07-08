In this binary version (3.9.2.0) the following features work:
*AWBlogListener is created by AWB and then sent through the article-processing process, including being sent to active plugins
*If the end result is to skip, the log entry gets added to the Ignored listview, with a reason
*If the page is saved, the log entry is added to the Saved listview
*All log entries have a tooltip with more info inside them

Broken in SVN (pretty much all of it) :
*Article log entries all get added to Saved list
*AWB doesn't skip in non bot-mode even if plugin tells it to skip (it skips in auto mode, however)
*No tooltips on log items

Note we're talking here about the Logs tab, NOT "Logging options" and logging to wiki/XHTML. That had seperate issues (trivial) which I've fixed, and needed a new upload solution based on AWBProfiles which Sam and I are working on.

To test with these binaries:
*Just run it. plugin is installed
*You can load 20th Century deaths s 6.xml and run that job for a while. Should be mostly or entirely skips as I think Sam has already done that bot job.
*20th Century deaths s 15.xml hopefully will include some article saves. If not, find a talk page category to test (Sam or I can advise)

--Kingboyk