public with sharing class clsFive9ListRemoveLeads {

    public class ActionRequest  {
    @InvocableVariable
    public ID LeadID;
   // @InvocableVariable
   // public Boolean RemovefromDialer;
  }
   
   
   @InvocableMethod
   public static void deleteLeadsFromList(List<ActionRequest> requests)
   { 
     
     List<String> lstdelLeads=new List<String>();
     
     for(ActionRequest requestObj:requests){ 
          //if(requestObj.RemovefromDialer==true)
          //{
              lstdelLeads.add(requestObj.LeadID);
         // }
      }
      //delete leads from dialer
      List<Five9LSP__Five9_List_Item__c> lstFive9ListItems=[Select id from Five9LSP__Five9_List_Item__c where Five9LSP__Lead__c =:lstdelLeads];
      if(lstFive9ListItems !=null && lstFive9ListItems.size()>0)
      {
        delete lstFive9ListItems;
      }
   }
 }