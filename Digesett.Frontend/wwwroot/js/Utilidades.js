function DatatableNotify(table)
{
    $(table).DataTable().destroy();
    const datatableOptions = {
        PageLength: 3,
        destroy: true,
        language: {
            lengthMenu: "Mostrar _MENU_ registros por pagina",
            zeroRecord: "No hay data que mostrar...",
            info: "Mostrando _START_ De _END_ de un total de _TOTAL_ registros",
            infoEmpty: "No hay data que mostrar...",
            search: "Buscar:",
            infoFiltered: "(filtrado desde _MAX_ registros totales)",
            loadingRecord: "Cargando...",
            paginate: {
                first: "Primero",
                last: "Ultimo",
                next: "Siguiente",
                previous: "Anterior"
            }
        },
        'rowCallback': function (fila, datos, indice) {
            if (datos[14] == 'True') {
                $(fila).find('td').addClass('lightRed')
            }
        }



    }

    $(document).ready(function () {
        $(table).DataTable(datatableOptions);
    });
    
};


function SetFormatStyleDatatable(table)
{
    $(document).ready(function () {
        let dtponches = new Datatable(table, {
            "columnDefs": [
                {
                    "targets": 1,
                    render: function (data,type,full,meta) {
                        console.log(data);
                    }
                }
            ]
        });
    });
}



function NotificationTaskOnCompleted(response)
{
    if (response) {
        Swal.fire({
            title: "Advertencia",
            text: "La tarea se completo correctamente",
            icon: 'info',
            confirmButtonText: "Ok",
        });
    } else
    {
        Swal.fire({
            title: "Error",
            text: "error al completar la tarea correctamente",
            icon: 'error',
            confirmButtonText: "Ok",
        });
    }
}

function ListaEmpleadosExcepctionOut(lista)
{   
    lista.forEach(item => {
        const fecha = new Date(item.exceptionDateEnd);
        const dia = fecha.getDate();
        const mes = fecha.getMonth()+1;
        const año = fecha.getFullYear();
        const fechaFormateada = dia + "-" + mes + "-" + año;
        item.exceptionDateEnd = fechaFormateada;
    }); 
    const numrecord = lista.length;

    let htmlContent = '<table class="table table-bordered table-condensed table-striped table-sm shadow" id="mitable"><thead><tr><th>Id</th><th>nombre</th><th>Salida</th></tr></thead><tbody>';
    lista.forEach(item => {
        htmlContent += `<tr><td>${item.cod_empleado}</td><td>${item.nombre_empleado}</td><td>${item.exceptionDateEnd}</td><td><input type="checkbox" class="checkbox" checked></td></tr>`;
    });
    htmlContent += `</tbody></table></br><p>numero de registro: ${numrecord}</p><p>Puede revisar estos datos en el bioadmin, la aplicacion registrara los usuarios tildados y los actualizará en el bioadmin</p>`;
    console.log(lista);

    Swal.fire({
        title: "Lista de empleados con periodo vencido.",
        html: htmlContent,
        icon: 'info',
        width: '600px',
        showCancelButton: true,
        confirmButtonText: "Procesar",
    }).then((result) => {
        if (result.isConfirmed)
        {
            const checkboxes = document.querySelectorAll('input[type="checkbox"]');
            Swal.fire({
                title: 'Realmente desea actualizar estos registrtos...?',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: `<i class="fa fa-thumbs-up"></i> Si, acepto`,
                cancelButtonText: 'No, cancelo.'
            }).then((result) => {
                if (result.isConfirmed) {
                    console.log("Ejecucion de sp para actualizar los datos...");
                   
                    const registrosSeleccionados = [];

                    checkboxes.forEach(checkbox => {
                        if (checkbox.checked) {
                            const row = checkbox.closest('tr');
                            console.log(row);
                            const cod_empleado = row.cells[0].textContent;
                            const nombre_empleado = row.cells[1].textContent;
                           
                            

                            registrosSeleccionados.push({ cod_empleado });
                        }        
                    });
                    console.log('Registros Seleccionados: ', registrosSeleccionados);

                    //actualizar la lista de empleados en el bioadmin.
                    DotNet.invokeMethodAsync('Digesett.Frontend', 'ActualizarPeriodos',registrosSeleccionados)
                        .then(() => {
                            console.log("actualizar bio-admin...");
                        });
                } else if (result.dismiss === Swal.DismissReason.cancel)
                {
                    console.log("no se hace nada...")
                }           
            });

        } else if (result.dismiss === Swal.DismissReason.cancel)
        {
            Swal.fire("Cambios no confirmados", "", "info");
        }
    });
}

function saveAsFile(filename, bytesBase64)
{
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

window.saveExcel = function (filename, data) {
    const blob = new Blob([data], { type: 'application/octet-stream' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);

};

window.GenerarPDF = async (date,hora,data,tdesde,thasta) => {
    const pdf = new jsPDF();
    pdf.setFontSize(10);
    pdf.setFont("helvetica");
    //encabezado del reporte
    pdf.text(5, 5, "Direccion General Digesett");
    pdf.text(170, 5, "Fecha: " + date);
    pdf.text(5, 10, "Aplicacion Control de Asistencia");
    pdf.text(170, 10, "Hora: " + hora);
    pdf.text(5, 15, "Sede - Santo Domingo");
    pdf.text(170, 15, "Pagina: " + pdf.internal.getNumberOfPages());
    pdf.setFontSize(18);
    pdf.text(60, 20, "Reporte General de Asistencia").setFont(undefined, 'bold');
    pdf.setFontSize(10);
    pdf.text(60, 25, "Parametros de Fecha:[ Desde: "+tdesde+" - " + thasta + " ]");
    const pdfWidth = pdf.internal.pageSize.getWidth;
    const pdfHeight = pdf.internal.pageSize.getHeight;
    pdf.line(0, 60, pdfWidth, 60);
    var img = new Image;
    img.crossOrigin = "";
    img.onload = function () {
    }
    //cuerpo del reporte
    var datos = JSON.parse(data); 
    pdf.setFontSize(8);

    pdf.text(1, 30, "ID. USER").setFont(undefined, 'bold');
    pdf.text(15, 30, "NOMBRE DEL EMPLEADO").setFont(undefined, 'bold');
    pdf.text(55, 30, "DEPARTAMENTO").setFont(undefined, 'bold');
    pdf.text(95, 30, "CARGO").setFont(undefined, 'bold');
    pdf.text(125, 30, "FECHA").setFont(undefined, 'bold');
    pdf.text(145, 30, "DIA").setFont(undefined, 'bold');
    pdf.text(160, 30, "ENTRADA").setFont(undefined, 'bold');
    pdf.text(180, 30, "SALIDA").setFont(undefined, 'bold');


  
    let col = 35;
    datos.forEach((item, index) => {
        pdf.text(3, col, String(item.Id)).setFont(undefined, 'normal');
        pdf.text(15, col, String(item.Empleado).substr(0, 20)).setFont(undefined, 'normal');
        pdf.text(55, col, String(item.Departamento).substr(0, 20)).setFont(undefined, 'normal');
        pdf.text(95, col, String(item.Cargo).substr(0, 15)).setFont(undefined, 'normal');
        pdf.text(125, col, String(item.RecordTime).substr(0, 10)).setFont(undefined, 'normal');
        pdf.text(145, col, String(item.StringNameDate).substr(0, 10)).setFont(undefined, 'normal');
        pdf.text(160, col, String(item.Marca1).substr(0, 10)).setFont(undefined, 'normal');
        pdf.text(180, col, String(item.Marca2).substr(0, 10)).setFont(undefined, 'normal');
        col += 4;
        if (col > 280)
        {
            pdf.line(5, 285, 200, 285);
            pdf.addPage();
            pdf.text(5, 5, "Direccion General Digesett");
            pdf.text(170, 5, "Fecha: " + date);
            pdf.text(5, 10, "Aplicacion Control de Asistencia");
            pdf.text(170, 10, "Hora: " + hora);
            pdf.text(5, 15, "Santo Domingo");
            pdf.text(170, 15, "Pagina: " + pdf.internal.getNumberOfPages());
            pdf.setFontSize(18);
            pdf.text(60, 20, "Reporte General de Asistencia").setFont(undefined, 'bold');
            pdf.setFontSize(8);

            pdf.text(1, 30, "ID. USER").setFont(undefined, 'bold');
            pdf.text(15, 30, "NOMBRE DEL EMPLEADO").setFont(undefined, 'bold');
            pdf.text(55, 30, "DEPARTAMENTO").setFont(undefined, 'bold');
            pdf.text(95, 30, "CARGO").setFont(undefined, 'bold');
            pdf.text(125, 30, "FECHA").setFont(undefined, 'bold');
            pdf.text(145, 30, "DIA").setFont(undefined, 'bold');
            pdf.text(160, 30, "ENTRADA").setFont(undefined, 'bold');
            pdf.text(180, 30, "SALIDA").setFont(undefined, 'bold');
            col = 35;
        }
       
    });
    console.log(col);

    
    //pdf.text(10, 45, String(datos[0].Empleado));
    //pdf.text(53, 45, String(datos[0].Departamento));
    //pdf.text(115, 45, String(datos[0].Cargo));
    //pdf.text(140, 45, String(datos[0].RecordTime));
    //pdf.text(170, 45, String(datos[0].Marca1));
    //pdf.text(190, 45, String(datos[0].Marca2));
    //guardar y descargar el reporte PDF.
    pdf.save("asistencia_digesett.pdf");
}; 

function DataTableInitPonches(table, jsonData) {
    DataTableLoad(table)
}

function DataTableLoad(table) {
    $(function () {
        $(table).DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _START_-_END_ (_TOTAL_ Filas)",
                "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar Por:",
                "zeroRecords": "Sin resultados encontrados",
                "paginate": {
                    "first": "Primero",
                    "last": "Ultimo",
                    "next": "Siguiente",
                    "previous": "Anterior"
                }
            },
            pageLength: 10,
            destroy: true,
            lengthMenu: [5, 10, 20, 30, 40, 50]
        });
    });
    
}
function DataTableUnload(table) {
    
    $(table).DataTable().destroy();
   
}

function DataTableRepaint(table,jsonData)
{ 
    $(table + '_wrapper').remove();
    DataTableLoad(table);
};  