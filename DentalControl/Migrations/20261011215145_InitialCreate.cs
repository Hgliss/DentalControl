using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalControl.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificate_template",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificate_template", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false),
                    code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    order_index = table.Column<int>(type: "INTEGER", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_catalog_item_catalog_category_category_id",
                        column: x => x.category_id,
                        principalTable: "catalog_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "charge",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    consultation_id = table.Column<int>(type: "INTEGER", nullable: true),
                    treatment_id = table.Column<int>(type: "INTEGER", nullable: true),
                    description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    amount = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    charge_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_charge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "consultation",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    treatment_id = table.Column<int>(type: "INTEGER", nullable: true),
                    consultation_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    consultation_type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    reason_for_visit = table.Column<string>(type: "TEXT", nullable: false),
                    systolic_pressure = table.Column<int>(type: "INTEGER", nullable: true),
                    diastolic_pressure = table.Column<int>(type: "INTEGER", nullable: true),
                    heart_rate = table.Column<int>(type: "INTEGER", nullable: true),
                    oxygen_saturation = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    temperature = table.Column<decimal>(type: "TEXT", precision: 4, scale: 2, nullable: true),
                    physical_notes = table.Column<string>(type: "TEXT", nullable: true),
                    diagnosis_description = table.Column<string>(type: "TEXT", nullable: true),
                    procedure_description = table.Column<string>(type: "TEXT", nullable: true),
                    medical_observations = table.Column<string>(type: "TEXT", nullable: true),
                    requires_follow_up = table.Column<bool>(type: "INTEGER", nullable: false),
                    next_appointment = table.Column<DateTime>(type: "TEXT", nullable: true),
                    consultation_cost = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medical_certificate",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    consultation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    template_id = table.Column<int>(type: "INTEGER", nullable: true),
                    issue_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_certificate", x => x.id);
                    table.ForeignKey(
                        name: "FK_medical_certificate_certificate_template_template_id",
                        column: x => x.template_id,
                        principalTable: "certificate_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_medical_certificate_consultation_consultation_id",
                        column: x => x.consultation_id,
                        principalTable: "consultation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    first_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    second_last_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    gender_id = table.Column<int>(type: "INTEGER", nullable: false),
                    phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    cui = table.Column<string>(type: "TEXT", maxLength: 13, nullable: true),
                    occupation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    referred_by = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.patient_id);
                    table.ForeignKey(
                        name: "FK_patient_catalog_item_gender_id",
                        column: x => x.gender_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_allergy",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    allergen_id = table.Column<int>(type: "INTEGER", nullable: true),
                    other_allergen_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    reaction = table.Column<string>(type: "TEXT", nullable: true),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_allergy", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_allergy_catalog_item_allergen_id",
                        column: x => x.allergen_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_allergy_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_condition",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    condition_id = table.Column<int>(type: "INTEGER", nullable: true),
                    other_condition_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    diagnosis_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    clinical_status_id = table.Column<int>(type: "INTEGER", nullable: true),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_condition", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_condition_catalog_item_clinical_status_id",
                        column: x => x.clinical_status_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_condition_catalog_item_condition_id",
                        column: x => x.condition_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_condition_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    history_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    event_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_history_catalog_item_history_type_id",
                        column: x => x.history_type_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_history_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_medication",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    medicine_id = table.Column<int>(type: "INTEGER", nullable: true),
                    other_medicine_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    dosage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    frequency = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    administration_route = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    is_current = table.Column<bool>(type: "INTEGER", nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_medication", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_medication_catalog_item_medicine_id",
                        column: x => x.medicine_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_medication_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    payment_method_id = table.Column<int>(type: "INTEGER", nullable: false),
                    amount_paid = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    payment_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    receipt_number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    invoice_requested = table.Column<bool>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_catalog_item_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payment_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    payment_id = table.Column<int>(type: "INTEGER", nullable: false),
                    charge_id = table.Column<int>(type: "INTEGER", nullable: false),
                    amount_applied = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_detail_charge_charge_id",
                        column: x => x.charge_id,
                        principalTable: "charge",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payment_detail_payment_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prescription",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    consultation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    prescription_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescription", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescription_consultation_consultation_id",
                        column: x => x.consultation_id,
                        principalTable: "consultation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prescription_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prescription_id = table.Column<int>(type: "INTEGER", nullable: false),
                    medicine_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    dosage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    frequency = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    duration = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    additional_instructions = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescription_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescription_item_prescription_prescription_id",
                        column: x => x.prescription_id,
                        principalTable: "prescription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    role_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_user_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "treatment",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    patient_id = table.Column<int>(type: "INTEGER", nullable: false),
                    treatment_type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    total_cost = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    estimated_session_count = table.Column<int>(type: "INTEGER", nullable: true),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    treatment_status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    progress_notes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment", x => x.id);
                    table.ForeignKey(
                        name: "FK_treatment_catalog_item_treatment_status_id",
                        column: x => x.treatment_status_id,
                        principalTable: "catalog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_treatment_patient_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_treatment_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_treatment_user_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_treatment_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_category_code",
                table: "catalog_category",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_category_created_by",
                table: "catalog_category",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_category_deleted_by",
                table: "catalog_category",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_category_updated_by",
                table: "catalog_category",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_item_category_id_code",
                table: "catalog_item",
                columns: new[] { "category_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_item_created_by",
                table: "catalog_item",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_item_deleted_by",
                table: "catalog_item",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_item_updated_by",
                table: "catalog_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_charge_consultation_id",
                table: "charge",
                column: "consultation_id");

            migrationBuilder.CreateIndex(
                name: "IX_charge_created_by",
                table: "charge",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_charge_deleted_by",
                table: "charge",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_charge_patient_id",
                table: "charge",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_charge_treatment_id",
                table: "charge",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_charge_updated_by",
                table: "charge",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_created_by",
                table: "consultation",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_deleted_by",
                table: "consultation",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_patient_id",
                table: "consultation",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_treatment_id",
                table: "consultation",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_updated_by",
                table: "consultation",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_medical_certificate_consultation_id",
                table: "medical_certificate",
                column: "consultation_id");

            migrationBuilder.CreateIndex(
                name: "IX_medical_certificate_created_by",
                table: "medical_certificate",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_medical_certificate_deleted_by",
                table: "medical_certificate",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_medical_certificate_template_id",
                table: "medical_certificate",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_medical_certificate_updated_by",
                table: "medical_certificate",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_created_by",
                table: "patient",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_cui",
                table: "patient",
                column: "cui",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_deleted_by",
                table: "patient",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_gender_id",
                table: "patient",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_patient_code",
                table: "patient",
                column: "patient_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_updated_by",
                table: "patient",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergy_allergen_id",
                table: "patient_allergy",
                column: "allergen_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergy_created_by",
                table: "patient_allergy",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergy_deleted_by",
                table: "patient_allergy",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergy_patient_id",
                table: "patient_allergy",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergy_updated_by",
                table: "patient_allergy",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_clinical_status_id",
                table: "patient_condition",
                column: "clinical_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_condition_id",
                table: "patient_condition",
                column: "condition_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_created_by",
                table: "patient_condition",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_deleted_by",
                table: "patient_condition",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_patient_id",
                table: "patient_condition",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_condition_updated_by",
                table: "patient_condition",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_history_created_by",
                table: "patient_history",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_history_deleted_by",
                table: "patient_history",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_history_history_type_id",
                table: "patient_history",
                column: "history_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_history_patient_id",
                table: "patient_history",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_history_updated_by",
                table: "patient_history",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_medication_created_by",
                table: "patient_medication",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_medication_deleted_by",
                table: "patient_medication",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_medication_medicine_id",
                table: "patient_medication",
                column: "medicine_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_medication_patient_id",
                table: "patient_medication",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_medication_updated_by",
                table: "patient_medication",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_created_by",
                table: "payment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_deleted_by",
                table: "payment",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_patient_id",
                table: "payment",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_payment_method_id",
                table: "payment",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_receipt_number",
                table: "payment",
                column: "receipt_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_updated_by",
                table: "payment",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_detail_charge_id",
                table: "payment_detail",
                column: "charge_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_detail_created_by",
                table: "payment_detail",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_detail_deleted_by",
                table: "payment_detail",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_detail_payment_id",
                table: "payment_detail",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_detail_updated_by",
                table: "payment_detail",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_consultation_id",
                table: "prescription",
                column: "consultation_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_created_by",
                table: "prescription",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_deleted_by",
                table: "prescription",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_updated_by",
                table: "prescription",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_item_created_by",
                table: "prescription_item",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_item_deleted_by",
                table: "prescription_item",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_item_prescription_id",
                table: "prescription_item",
                column: "prescription_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_item_updated_by",
                table: "prescription_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_role_created_by",
                table: "role",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_role_deleted_by",
                table: "role",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_role_name",
                table: "role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_updated_by",
                table: "role",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_created_by",
                table: "treatment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_deleted_by",
                table: "treatment",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_patient_id",
                table: "treatment",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_treatment_status_id",
                table: "treatment",
                column: "treatment_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_updated_by",
                table: "treatment",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_created_by",
                table: "user",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_deleted_by",
                table: "user",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_updated_by",
                table: "user",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_category_user_created_by",
                table: "catalog_category",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_category_user_deleted_by",
                table: "catalog_category",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_category_user_updated_by",
                table: "catalog_category",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_item_user_created_by",
                table: "catalog_item",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_item_user_deleted_by",
                table: "catalog_item",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_item_user_updated_by",
                table: "catalog_item",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_consultation_consultation_id",
                table: "charge",
                column: "consultation_id",
                principalTable: "consultation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_patient_patient_id",
                table: "charge",
                column: "patient_id",
                principalTable: "patient",
                principalColumn: "patient_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_treatment_treatment_id",
                table: "charge",
                column: "treatment_id",
                principalTable: "treatment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_user_created_by",
                table: "charge",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_user_deleted_by",
                table: "charge",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_charge_user_updated_by",
                table: "charge",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_consultation_patient_patient_id",
                table: "consultation",
                column: "patient_id",
                principalTable: "patient",
                principalColumn: "patient_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_consultation_treatment_treatment_id",
                table: "consultation",
                column: "treatment_id",
                principalTable: "treatment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_consultation_user_created_by",
                table: "consultation",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_consultation_user_deleted_by",
                table: "consultation",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_consultation_user_updated_by",
                table: "consultation",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_medical_certificate_user_created_by",
                table: "medical_certificate",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_medical_certificate_user_deleted_by",
                table: "medical_certificate",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_medical_certificate_user_updated_by",
                table: "medical_certificate",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_user_created_by",
                table: "patient",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_user_deleted_by",
                table: "patient",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_user_updated_by",
                table: "patient",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_allergy_user_created_by",
                table: "patient_allergy",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_allergy_user_deleted_by",
                table: "patient_allergy",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_allergy_user_updated_by",
                table: "patient_allergy",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_condition_user_created_by",
                table: "patient_condition",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_condition_user_deleted_by",
                table: "patient_condition",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_condition_user_updated_by",
                table: "patient_condition",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_history_user_created_by",
                table: "patient_history",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_history_user_deleted_by",
                table: "patient_history",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_history_user_updated_by",
                table: "patient_history",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_medication_user_created_by",
                table: "patient_medication",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_medication_user_deleted_by",
                table: "patient_medication",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_medication_user_updated_by",
                table: "patient_medication",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_user_created_by",
                table: "payment",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_user_deleted_by",
                table: "payment",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_user_updated_by",
                table: "payment",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_detail_user_created_by",
                table: "payment_detail",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_detail_user_deleted_by",
                table: "payment_detail",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_detail_user_updated_by",
                table: "payment_detail",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_user_created_by",
                table: "prescription",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_user_deleted_by",
                table: "prescription",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_user_updated_by",
                table: "prescription",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_item_user_created_by",
                table: "prescription_item",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_item_user_deleted_by",
                table: "prescription_item",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prescription_item_user_updated_by",
                table: "prescription_item",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_user_created_by",
                table: "role",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_user_deleted_by",
                table: "role",
                column: "deleted_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_user_updated_by",
                table: "role",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_user_created_by",
                table: "role");

            migrationBuilder.DropForeignKey(
                name: "FK_role_user_deleted_by",
                table: "role");

            migrationBuilder.DropForeignKey(
                name: "FK_role_user_updated_by",
                table: "role");

            migrationBuilder.DropTable(
                name: "medical_certificate");

            migrationBuilder.DropTable(
                name: "patient_allergy");

            migrationBuilder.DropTable(
                name: "patient_condition");

            migrationBuilder.DropTable(
                name: "patient_history");

            migrationBuilder.DropTable(
                name: "patient_medication");

            migrationBuilder.DropTable(
                name: "payment_detail");

            migrationBuilder.DropTable(
                name: "prescription_item");

            migrationBuilder.DropTable(
                name: "certificate_template");

            migrationBuilder.DropTable(
                name: "charge");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "prescription");

            migrationBuilder.DropTable(
                name: "consultation");

            migrationBuilder.DropTable(
                name: "treatment");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "catalog_item");

            migrationBuilder.DropTable(
                name: "catalog_category");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
