

//-------------------------------ListRoomCatgory---------------------------------------
searchModel = {
    CurrentPage: 1,
    RowPerPage: $('[name=dynamic-table_length]').val(),
    Keyword: $('#keyword').val(),
};

const searchRoomCategory = function () {
    searchModel.CurrentPage = 1;
    searchModel.PageSize = $('[name=dynamic-table_length]').val();
    searchModel.Keyword = $('#keyword').val();

    getdataListRoomCategory(searchModel);
}

const getdataListRoomCategory = async function (searchModel) {
    let rq = await fetch('/api/AdminRoomCategory/GetListRoomCategory?keyword=' + searchModel.Keyword + '&page=' + searchModel.CurrentPage + '&pageSize=' + searchModel.PageSize, {
        method: 'get',
        headers: Enum.OptionAdminHeaderDefault
    });
    let rs = await rq.json();
    if (AdminCheckErrorResponse(rs) === false) return;

    console.log(rs);

    if (AdminCheckErrorResponse(rs) === false) return;
    $('#list-data').html('');
    if (GetObjectProperty(rs, 'status') === Enum.ResponseStatus.SUCCESS) {
        let html = '';
        for (let i = 0; i < rs.data.List.length; i++) {
            html += '<tr>';
            html += '<td class="text-center"><span class="fs-15 font-w500 text-nowrap">' + (i + 1) + '</span></td>';
            html += '<td> <div class="guest-bx">';
            html += `<img class="me-4" src="${rs.data.List[i].Image}" alt="">`;
            html += `<div><h4 class="mb-0 mt-1 fs-16 font-w500 text-nowrap">${rs.data.List[i].Name}</h4>`
            html += `<span class="text-primary fs-15 font-w400 text-nowrap">${rs.data.List[i].View}</span></div> </div></td>`;
            html += `<td class="text-center"><div><span class="fs-15 font-w500 text-nowrap">${rs.data.List[i].NumberOfPeople}</span></div></td>`;
            html += `<td class="text-center"><div><span class="fs-15 font-w500 text-nowrap">${rs.data.List[i].SingleBed}</span></div></td>`;
            html += `<td class="text-center"><div><span class="fs-15 font-w500 text-nowrap">${rs.data.List[i].DoubleBed}</span></div></td>`;
            html += `<td class="text-center"><div><span class="fs-15 font-w500 text-nowrap">${rs.data.List[i].Square} m2</span></div></td>`;
            html += `<td><div><span class="fs-15 font-w500 text-nowrap">${NumberFormat(rs.data.List[i].Price)} VNĐ</span></div></td>`;
            html += `<td><div><span class="fs-15 font-w500 text-nowrap">${DateStringFormat({ stringDate: new Date(rs.data.List[i].CreateTime), newFormat: 'dd/mm/yyyy hh:mi:ss' })}</span></div></td>`;
            html += `<td><div class="dropdown dropstart"><a href="" class="btn-link" data-bs-toggle="dropdown" aria-expanded="false"><i class="fas fa-ellipsis-h"></i></a>`;
            html += `<div class="dropdown-menu">
                    <button class="dropdown-item fs-15" data-id="${rs.data.List[i].RoomCategoryId}">Edit</button>
                    <button class="dropdown-item fs-15" data-id="${rs.data.List[i].RoomCategoryId}">Delete</button>
                    <button class="dropdown-item fs-15" data-id="${rs.data.List[i].RoomCategoryId}">Trạng thái</button>
                </div></div></td>`;
            html += `</tr>`;
        }

        $('#list-data').append(html);
        Pagination(searchModel, rs.data.TotalPage, null, getdataListRoomCategory);
    }
}

const AddRoomCategory_onClick = function () {
    window.location.href = ('/admin/RoomCategory/AddRoomCategory/');
}



const searchKeyPress = function (event) {
    if (event.keyCode == 13) {
        searchRoomCategory();
    }
}

const initPage = function () {
    searchRoomCategory();
}
//-----------------------------EndListRoomCategory---------------------------------------