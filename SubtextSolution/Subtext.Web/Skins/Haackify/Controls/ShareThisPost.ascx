<%@ Control Language="c#" AutoEventWireup="false" Inherits="Subtext.Web.UI.Controls.CurrentEntryControl" %>
<div class="share">
	<span>Share this Post: </span>
	<ul>
		<li>
			<a href="mailto:?body=Thought+you+would+find+this+interesting.+<%#UrlEncode(EntryFullyQualifedUrl)%>&amp;subject=<%# UrlEncode(Entry.Title) %>" title="Email it">email it</a>
		</li>
		<li>
			<a href="http://del.icio.us/login?url=<%# UrlEncode(EntryFullyQualifedUrl) %>;title=<%# UrlEncode(Entry.Title) %>" title="Bookmark it at del.icio.us">bookmark It</a>
		</li>
		<li>
			<a href="http://digg.com/submit?url=<%# UrlEncode(EntryFullyQualifedUrl) %>&amp;phase=2" title="digg it">digg It</a>
		</li>
		<li>
			<a href="http://reddit.com/submit?url=<%# UrlEncode(EntryFullyQualifedUrl) %>&amp;title=<%# UrlEncode(Entry.Title) %>" title="redd">redd It</a>
		</li>
	</ul>
</div>