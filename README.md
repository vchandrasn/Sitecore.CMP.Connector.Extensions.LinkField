# Sitecore.CMP.Connector.Extensions.LinkField #
This module extends [Sitecore Connect™ for Sitecore CMP 2.0.0](https://dev.sitecore.net/Downloads/Sitecore_Connect_for_Sitecore_CMP/20/Sitecore_Connect_for_Sitecore_CMP_200.aspx) so link fields (title and link) set on CMP entities can be synchronised with Generic Link fields in Sitecore XP.
This module was built against Sitecore 9.1 Update 1.

## Installation Instructions ##
- Install [Sitecore Connect™ for Sitecore CMP 2.0.0](https://dev.sitecore.net/Downloads/Sitecore_Connect_for_Sitecore_CMP/20/Sitecore_Connect_for_Sitecore_CMP_200.aspx).
- Install package inside items folder. It contains one template and adds the template to standard values of CMP entity mapping item.
- Place the appropriate dll files in libraries folder as mentioned in dlls_to_place_in_this_folder.txt
- Deploy code.

## Usage ##
- Create an Link Field Mapping item and populate the fields
    - 'CMP Link Label Field Name' is the field in CMP that contains the link text.
    - 'CMP Link Field Name' is the field in CMP that contains the actual link.
        - This module works for both internal and external links.
    - 'Sitecore Field Name' is the sitecore generic link field to map to.