function exportData(id, fileName) {

	var gridid = $(id).getDataIDs(); // Get all the ids in array
	var label = $(id).jqGrid('getGridParam', 'colNames');

	var obj = new Object();
	obj.count = gridid.length;


	obj.items = new Array();
	for (elem in gridid) {
		obj.items.push($(id).getRowData(gridid[elem]));
	}
	var json = JSON.stringify(obj);
	JSONToCSVConvertor(json, label, fileName);

}


function JSONToCSVConvertor(JSONData, label, fileName) {

	//If JSONData is not an object then JSON.parse will parse the JSON string in an Object
	var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;
	var CSV = '\uFEFF';
	//This condition will generate the Label/Header
	if (label) {
		var row = "";

		//This loop will extract the label from 1st index of on array
		for (var index in label) {
			//Now convert each value to string and comma-seprated
			row += label[index] + ',';
		}
		row = row.slice(0, -1);
		//append Label row with line break
		CSV += row + '\r\n';
	}

	//1st loop is to extract each row
	for (var i = 0; i < arrData.items.length; i++) {
		var row = "";
		//2nd loop will extract each column and convert it in string comma-seprated
		for (var index in arrData.items[i]) {
			row += '"' + arrData.items[i][index].replace(/(<([^>]+)>)/ig, '') + '",';
		}
		row.slice(0, row.length - 1);
		//add a line break after each row
		CSV += row + '\r\n';
	}

    if (CSV === '\uFEFF') {
        alert("No data to export!");
        return;
    }

	/*
	 * 
	 * FORCE DOWNLOAD
	 * 
	 */


	//this trick will generate a temp "a" tag
	var link = document.createElement("a");
	link.id = "lnkDwnldLnk";

	//this part will append the anchor tag and remove it after automatic click
	document.body.appendChild(link);

	var csv = CSV;
	blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });

	var myURL = window.URL || window.webkitURL;

	var csvUrl = myURL.createObjectURL(blob);
	var filename = fileName + '.csv';
	jQuery("#lnkDwnldLnk")
		.attr({
			'download': filename,
			'href': csvUrl
		});

	jQuery('#lnkDwnldLnk')[0].click();
	document.body.removeChild(link);
}
