USE OPDClinicDB;
GO

CREATE SCHEMA reporting;
GO

/* ============================================================
   REPORTING ENGINE - LEAN VERSION
   ============================================================ */


/* ============================================================
   1. REPORT
   ============================================================ */

create table reporting.report (
    report_id          bigint          not null,
    report_code        varchar(100)    not null,
    report_name        varchar(200)    not null,
    description        varchar(1000)   null,

    /*
     * Logical name of the DAL method/provider.
     *
     * Example:
     *     sales_summary
     *     customer_aging
     *     inventory_summary
     */
    data_source_code   varchar(100)    not null,

    /*
     * Examples:
     *     tabular
     *     summary
     */
    report_type        varchar(30)     not null,

    is_active          smallint        not null default 1,

    constraint pk_report primary key (report_id),
    constraint uq_report_code unique (report_code),
    constraint ck_report_active check (is_active in (0, 1))
);


/* ============================================================
   2. REPORT COLUMN
   ============================================================ */

create table reporting.report_column (
    report_column_id   bigint          not null,
    report_id          bigint          not null,

    /*
     * Name of the field returned by the DAL.
     *
     * Example:
     *     customer_name
     *     invoice_count
     *     net_amount
     */
    column_code        varchar(100)    not null,
    display_label      varchar(200)    not null,

    /*
     * Examples:
     *     string
     *     integer
     *     decimal
     *     date
     *     datetime
     *     boolean
     */
    data_type          varchar(30)     not null,
    display_order      integer         not null,
    is_visible         smallint        not null default 1,

    /*
     * Optional formatting.
     *
     * Examples:
     *     yyyy-mm-dd
     *     #,##0.00
     */
    format_pattern     varchar(100)    null,

    constraint pk_report_column primary key (report_column_id),
    constraint uq_report_column_code unique (report_id, column_code),
    constraint fk_report_column_report foreign key (report_id) references reporting.report (report_id) on delete cascade,
    constraint ck_report_column_visible check (is_visible in (0, 1)),
    constraint ck_report_column_order check (display_order >= 0)
);

/* ============================================================
   3. REPORT FILTER
   ============================================================ */

create table reporting.report_filter (
    report_filter_id   bigint          not null,
    report_id          bigint          not null,

    /* Logical filter identifier. Example: date_from, date_to, customer_id */
    filter_code        varchar(100)    not null,
    filter_label       varchar(200)    not null,

    /* Parameter name passed to the DAL */
    parameter_name     varchar(100)    not null,

    /* Examples: string, integer, decimal, date, datetime, boolean */
    data_type          varchar(30)     not null,

    /* Examples: text, number, date, datetime, select, multi_select, checkbox, lookup */
    control_type       varchar(30)     not null,
    lookup_code        varchar(100)    null,
    default_value      varchar(500)    null,
    is_required        smallint        not null default 0,
    display_order      integer         not null,

    /* Examples: validation_rule = date_before_or_equal, validation_value = date_to */
    validation_rule varchar(50) NULL,
    validation_value varchar(100) NULL,    

    constraint pk_report_filter primary key (report_filter_id),
    constraint uq_report_filter_code unique (report_id, filter_code),
    constraint uq_report_filter_parameter unique (report_id, parameter_name),
    constraint fk_report_filter_report foreign key (report_id) references reporting.report (report_id) on delete cascade,
    constraint ck_report_filter_required check (is_required in (0, 1)),
    constraint ck_report_filter_order check (display_order >= 0)
);

/* ============================================================
   4. REPORT PERMISSION
   ============================================================ */

create table reporting.report_permission (
    report_permission_id  bigint        not null,
    report_id             bigint        not null,

    /*
     * Examples:
     *     user
     *     role
     *     group
     */
    principal_type        varchar(20)   not null,

    /*
     * ID from the application's security model.
     */
    principal_id          bigint        not null,

    /*
     * Examples:
     *     export
     *     print
     */
    permission_code       varchar(30)   not null,

    constraint pk_report_permission primary key (report_permission_id),
    constraint uq_report_permission unique (report_id, principal_type, principal_id, permission_code),
    constraint fk_report_permission_report foreign key (report_id) references reporting.report (report_id) on delete cascade,
    constraint ck_report_permission_principal_type check (principal_type in ('user', 'role', 'group')),
    constraint ck_report_permission_code check (permission_code in ('export', 'print'))
    constraint ck_report_permission_principal_id check (principal_id > 0)
);


/* ============================================================
   INDEXES
   ============================================================ */

create index ix_report_data_source_code on reporting.report (data_source_code);
create index ix_report_column_report on reporting.report_column (report_id, display_order);
create index ix_report_filter_report on reporting.report_filter (report_id, display_order);
create index ix_report_permission_principal on reporting.report_permission (principal_type, principal_id);