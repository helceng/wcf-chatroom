/***********************************************************************************************************
	 
	  Author:         Helceng
	  CreatedDate:    2017-01-23
	  Description:    chatroom program developed with WFC; help improve it if you are willingly to
	  Usage:
	                   Step1:  change app.config located in project Hosting,
					           modify 'address' attribute of node 'endpoint' to 'localhost' or '127.0.0.1' 
					   Step2:  run Hosting project as WCF hosting and offer server-side service
					   Step3:  update WCF reference 'SRcallback' in Client project with new ip address as  
  			                   you  configured in step1, so the app.config in client-side will be updated
					   Step4:  run Client project    
					   
************************************************************************************************************/